using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;

namespace SIA.AcademicService.Infrastructure.MessageBus.Publishers
{
  public sealed class OutboxPublisherService : BackgroundService
  {
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
        catch (Exception exception)
        {
          _logger.LogError(
              exception,
              "Ocurrió un error al procesar los mensajes Outbox.");
        }

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
      }
    }

    private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
      using var scope = _scopeFactory.CreateScope();

      var dbContext =
          scope.ServiceProvider.GetRequiredService<AcademicDbContext>();

      var publishEndpoint =
          scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

      var pendingMessages =
          await dbContext.OutboxMessages
              .Where(message =>
                  message.ProcessedAtUtc == null &&
                  message.RetryCount < 5)
              .OrderBy(message => message.OccurredAtUtc)
              .Take(20)
              .ToListAsync(cancellationToken);

      foreach (var message in pendingMessages)
      {
        try
        {
          await PublishMessageAsync(message.EventType, message.Payload, publishEndpoint, cancellationToken);

          message.MarkAsProcessed();

          await dbContext.SaveChangesAsync(cancellationToken);

          _logger.LogInformation(
              "Evento {EventType} publicado correctamente. MessageId: {MessageId}",
              message.EventType,
              message.Id);
        }
        catch (Exception exception)
        {
          message.MarkAsFailed(exception.Message);

          await dbContext.SaveChangesAsync(cancellationToken);

          _logger.LogError(
              exception,
              "No se pudo publicar el evento {EventType}. MessageId: {MessageId}",
              message.EventType,
              message.Id);
        }
      }
    }

    private static async Task PublishMessageAsync(string eventType, string payload, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
    {
      if (eventType == $"{nameof(SubjectCreatedIntegrationEvent)}.v1")
      {
        var integrationEvent = JsonSerializer.Deserialize<SubjectCreatedIntegrationEvent>(payload);

        if (integrationEvent is null)
        {
            if (eventType == $"{nameof(SubjectCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<SubjectCreatedIntegrationEvent>(payload);

                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }

                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(AcademicPeriodCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<AcademicPeriodCreatedIntegrationEvent>(payload);

                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de periodo académico creado.");
                }

                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(AcademicPeriodUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<AcademicPeriodUpdatedIntegrationEvent>(payload);

                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de periodo académico actualizado.");
                }

                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(AcademicPeriodDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<AcademicPeriodDeactivatedIntegrationEvent>(payload);

                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de periodo académico desactivado.");
                }

                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(AcademicPeriodActivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<AcademicPeriodActivatedIntegrationEvent>(payload);

                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de periodo académico activado.");
                }

                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            throw new NotSupportedException($"El tipo de evento {eventType} no está soportado.");
        }

        await publishEndpoint.Publish(integrationEvent, cancellationToken);

        return;
      }
      if (eventType == $"{nameof(EducationalProgramCreatedIntegrationEvent)}.v1")
      {
        var integrationEvent = JsonSerializer.Deserialize<EducationalProgramCreatedIntegrationEvent>(payload);

        if (integrationEvent is null)
        {
            throw new InvalidOperationException("No fue posible deserializar el evento de programa educativo creado.");
        }

        await publishEndpoint.Publish(integrationEvent, cancellationToken);
        return;
      }

      if (eventType == $"{nameof(StudyPlanCreatedIntegrationEvent)}.v1")
      {
        var integrationEvent = JsonSerializer.Deserialize<StudyPlanCreatedIntegrationEvent>(payload);
        if (integrationEvent is null)
        {
            throw new InvalidOperationException("No fue posible deserializar el evento de plan de estudios creado.");
        }
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
        return;
      }

      throw new NotSupportedException(
          $"El tipo de evento {eventType} no está soportado.");
    }
}
