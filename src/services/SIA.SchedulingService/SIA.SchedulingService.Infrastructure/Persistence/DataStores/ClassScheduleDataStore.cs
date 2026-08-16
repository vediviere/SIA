using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class ClassScheduleDataStore : IClassScheduleDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public ClassScheduleDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClassSchedule?> GetClassScheduleByIdAsync(Guid tenantId, Guid classScheduleId, CancellationToken cancellationToken)
    {
        return await _dbContext.ClassSchedules
            .Include(x => x.ClassroomLab)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == classScheduleId, cancellationToken);
    }

    public async Task AddClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassScheduleCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.ClassSchedules.AddAsync(classSchedule, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassScheduleUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ClassSchedules.Update(classSchedule);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassScheduleDeletedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ClassSchedules.Update(classSchedule);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleRestoredIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassScheduleRestoredV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ClassSchedules.Update(classSchedule);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}