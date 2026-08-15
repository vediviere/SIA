using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.MessageBus.Publishers;

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
        // Eventos de Building (Edificios)
        if (eventType == $"{nameof(BuildingCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<BuildingCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(BuildingUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<BuildingUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(BuildingActivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<BuildingActivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio activado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(BuildingDeactivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<BuildingDeactivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio desactivado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Eventos de Group (Grupos)
        if (eventType == $"{nameof(GroupCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<GroupCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de grupo creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(GroupUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<GroupUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de grupo actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(GroupActivateIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<GroupActivateIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de grupo activado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(GroupDeactivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<GroupDeactivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de grupo desactivado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }


        //Eventos AcademicLoad
        if (eventType == $"{nameof(AcademicLoadCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicLoadCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(AcademicLoadUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicLoadUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(AcademicLoadActivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicLoadActivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio activado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(AcademicLoadDeactivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicLoadDeactivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio desactivado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }


        //Eventos AcademicOffering
        if (eventType == $"{nameof(AcademicOfferingCreatedIntegrationEvet)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicOfferingCreatedIntegrationEvet>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(AcademicOfferingUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicOfferingUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(AcademicOfferingActivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicOfferingActivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio activado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(AcademicOfferingDeactivatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<AcademicOfferingDeactivatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de edificio desactivado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Eventos de ClassroomType
        if (eventType == $"{nameof(ClassroomTypeCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomTypeCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de tipo de aula creada.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassroomTypeUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomTypeUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de tipo de aula actualizada.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassroomTypeDeletedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomTypeDeletedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de tipo de aula eliminada.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassroomTypeRestoredIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomTypeRestoredIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de tipo de aula restaurada.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Eventos de ClassroomLab 
        if (eventType == $"{nameof(ClassroomLabCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomLabCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de laboratorio creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassroomLabUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomLabUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de laboratorio actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassroomLabDeletedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomLabDeletedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de laboratorio eliminado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassroomLabRestoredIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassroomLabRestoredIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de laboratorio restaurado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Eventos de SupportSchedule 
        if (eventType == $"{nameof(SupportScheduleCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportScheduleCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(SupportScheduleUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportScheduleUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(SupportScheduleDeletedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportScheduleDeletedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario eliminado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(SupportScheduleRestoredIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportScheduleRestoredIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario restaurado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Eventos de ClassSchedule 
        if (eventType == $"{nameof(ClassScheduleCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassScheduleCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario de clase creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassScheduleUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassScheduleUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario de clase actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassScheduleDeletedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassScheduleDeletedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario de clase eliminado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(ClassScheduleRestoredIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<ClassScheduleRestoredIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de horario de clase restaurado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Eventos de SupportActivity
        if (eventType == $"{nameof(SupportActivityCreatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportActivityCreatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de SupportActivity creado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(SupportActivityUpdatedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportActivityUpdatedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de SupportActivity actualizado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(SupportActivityDeletedIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportActivityDeletedIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de SupportActivity eliminado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }
        if (eventType == $"{nameof(SupportActivityRestoredIntegrationEvent)}.v1")
        {
            var integrationEvent = JsonSerializer.Deserialize<SupportActivityRestoredIntegrationEvent>(payload);
            if (integrationEvent is null) throw new InvalidOperationException("No fue posible deserializar el evento de SupportActivity restaurado.");
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
            return;
        }

        throw new NotSupportedException($"El tipo de evento {eventType} no está soportado.");
    }
}