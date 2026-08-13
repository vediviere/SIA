using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicStaffService.Infrastructure.MessageBus.Publishers
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
                scope.ServiceProvider.GetRequiredService<AcademicStaffDbContext>();

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
            if (eventType == $"{nameof(TeacherCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<TeacherCreatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de profesor creado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(TeacherUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<TeacherUpdatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de profesor actualizado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(TeacherActivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<TeacherActivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de profesor activado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(TeacherDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<TeacherDeactivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de profesor desactivado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(DivisionHeadCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<DivisionHeadCreatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de responsable de división creado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(DivisionHeadUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<DivisionHeadUpdatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de responsable de división actualizado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(DivisionHeadActivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<DivisionHeadActivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de responsable de división activado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(DivisionHeadDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<DivisionHeadDeactivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de responsable de división desactivado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(PersonCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<PersonCreatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de persona creada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(PersonUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<PersonUpdatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de persona actualizada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(PersonActivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<PersonActivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de persona activada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(PersonDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<PersonDeactivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de persona desactivada.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(CoordinatorCreatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<CoordinatorCreatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de coordinador creado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(CoordinatorUpdatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<CoordinatorUpdatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de coordinador actualizado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(CoordinatorActivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<CoordinatorActivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de coordinador activado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            if (eventType == $"{nameof(CoordinatorDeactivatedIntegrationEvent)}.v1")
            {
                var integrationEvent = JsonSerializer.Deserialize<CoordinatorDeactivatedIntegrationEvent>(payload);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("No fue posible deserializar el evento de coordinador desactivado.");
                }
                await publishEndpoint.Publish(integrationEvent, cancellationToken);
                return;
            }

            throw new NotSupportedException($"El tipo de evento {eventType} no está soportado.");
        }
    }
}