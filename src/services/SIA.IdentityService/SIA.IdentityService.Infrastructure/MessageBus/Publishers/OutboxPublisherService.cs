using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Infrastructure.Persistence.Contexts;

namespace SIA.IdentityService.Infrastructure.MessageBus.Publishers;

public sealed class OutboxPublisherService : BackgroundService
{
  private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
  private const int BatchSize = 20;

  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILogger<OutboxPublisherService> _logger;

  public OutboxPublisherService(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherService> logger)
  {
    _scopeFactory = scopeFactory;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await PublishPendingMessagesAsync(stoppingToken);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        break;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Ocurrió un error al procesar los mensajes Outbox.");
      }

      await Task.Delay(PollingInterval, stoppingToken);
    }
  }

  private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
  {
    using var scope = _scopeFactory.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

    var pendingMessages = await dbContext.OutboxMessages
      .Where(message => message.ProcessedAtUtc == null)
      .OrderBy(message => message.RetryCount)
      .ThenBy(message => message.OccurredAtUtc)
      .Take(BatchSize)
      .ToListAsync(cancellationToken);

    foreach (var message in pendingMessages)
    {
      try
      {
        await PublishMessageAsync(message.EventType, message.Payload, publishEndpoint, cancellationToken);

        message.MarkAsProcessed();
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Evento {EventType} publicado correctamente. MessageId: {MessageId}", message.EventType, message.Id);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
        throw;
      }
      catch (Exception exception)
      {
        message.MarkAsFailed(exception.Message);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogError(exception, "No se pudo publicar el evento {EventType}. MessageId: {MessageId}", message.EventType, message.Id);
      }
    }
  }

  private static async Task PublishMessageAsync(string eventType, string payload, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
  {
    switch (eventType.Trim())
    {
      case UserIntegrationEventTypes.UserCreatedV1:
        var userCreated = JsonSerializer.Deserialize<UserCreatedIntegrationEvent>(payload)
          ?? throw new InvalidOperationException("No fue posible deserializar UserCreatedIntegrationEvent.");

        await publishEndpoint.Publish(userCreated, cancellationToken);
        break;

      case UserIntegrationEventTypes.UserRoleAssignedV1:
        var roleAssigned = JsonSerializer.Deserialize<UserRoleAssignedIntegrationEvent>(payload)
          ?? throw new InvalidOperationException("No fue posible deserializar UserRoleAssignedIntegrationEvent.");

        await publishEndpoint.Publish(roleAssigned, cancellationToken);
        break;

      case UserIntegrationEventTypes.UserRoleRevokedV1:
        var roleRevoked = JsonSerializer.Deserialize<UserRoleRevokedIntegrationEvent>(payload)
          ?? throw new InvalidOperationException("No fue posible deserializar UserRoleRevokedIntegrationEvent.");

        await publishEndpoint.Publish(roleRevoked, cancellationToken);
        break;

      case UserIntegrationEventTypes.PasswordChangedV1:
        var passwordChanged = JsonSerializer.Deserialize<PasswordChangedIntegrationEvent>(payload)
          ?? throw new InvalidOperationException("No fue posible deserializar PasswordChangedIntegrationEvent.");

        await publishEndpoint.Publish(passwordChanged, cancellationToken);
        break;

      default:
        throw new NotSupportedException($"El tipo de evento {eventType} no está soportado.");
    }
  }
}
