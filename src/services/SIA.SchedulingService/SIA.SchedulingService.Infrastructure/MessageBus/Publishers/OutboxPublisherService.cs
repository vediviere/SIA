using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.MessageBus.Publishers
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
                scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();

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
            if (eventType == $"{nameof(BuildingCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<BuildingCreatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(BuildingUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<BuildingUpdatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(BuildingActivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<BuildingActivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(BuildingDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<BuildingDeactivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(GroupCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<GroupCreatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(GroupUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<GroupUpdatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(GroupActivateIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<GroupActivateIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(GroupDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<GroupDeactivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de materia creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }
        }
    }
}
