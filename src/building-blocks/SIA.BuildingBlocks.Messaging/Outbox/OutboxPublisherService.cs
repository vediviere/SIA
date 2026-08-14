using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SIA.BuildingBlocks.Messaging.Outbox;

public sealed class OutboxPublisherService : BackgroundService
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly OutboxOptions _options;
  private readonly ILogger<OutboxPublisherService> _logger;

  public OutboxPublisherService(IServiceScopeFactory scopeFactory, OutboxOptions options, ILogger<OutboxPublisherService> logger)
  {
    _scopeFactory = scopeFactory;
    _options = options;
    _logger = logger;
    _options.Validate();
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await PublishPendingMessagesAsync(stoppingToken);
        await Task.Delay(_options.PollingInterval, stoppingToken);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        break;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Ocurrió un error al procesar los mensajes Outbox.");
        await Task.Delay(_options.PollingInterval, stoppingToken);
      }
    }
  }

  private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
  {
    using var scope = _scopeFactory.CreateScope();
    var store = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
    var publisher = scope.ServiceProvider.GetRequiredService<IOutboxEventPublisher>();
    var messages = await store.GetPendingAsync(DateTime.UtcNow, _options.BatchSize, cancellationToken);

    foreach (var message in messages)
    {
      try
      {
        await publisher.PublishAsync(message.EventType, message.Payload, cancellationToken);
        message.MarkAsProcessed();
        await store.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Evento {EventType} publicado correctamente. MessageId: {MessageId}", message.EventType, message.Id);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
        throw;
      }
      catch (Exception exception)
      {
        var nextRetryCount = message.RetryCount + 1;

        if (nextRetryCount >= _options.MaxRetryCount)
        {
          message.MarkAsDeadLettered(exception.Message);
          await store.SaveChangesAsync(cancellationToken);

          _logger.LogError(exception, "Mensaje Outbox enviado a cuarentena después de {RetryCount} intentos. EventType: {EventType}, MessageId: {MessageId}", message.RetryCount, message.EventType, message.Id);
          continue;
        }

        var retryDelay = _options.GetRetryDelay(nextRetryCount);
        message.MarkAsFailed(exception.Message, DateTime.UtcNow.Add(retryDelay));
        await store.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(exception, "Falló la publicación de {EventType}. Reintento {RetryCount}/{MaxRetryCount} programado para {NextAttemptAtUtc}. MessageId: {MessageId}", message.EventType, message.RetryCount, _options.MaxRetryCount, message.NextAttemptAtUtc, message.Id);
      }
    }
  }
}
