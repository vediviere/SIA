using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class SupportScheduleDataStore : ISupportScheduleDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public SupportScheduleDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SupportSchedule?> GetSupportScheduleByIdAsync(Guid tenantId, Guid supportScheduleId, CancellationToken cancellationToken)
    {
        return _dbContext.SupportSchedules
            .Include(supportSchedule => supportSchedule.ClassroomLab)
            .FirstOrDefaultAsync(supportSchedule => supportSchedule.TenantId == tenantId && supportSchedule.Id == supportScheduleId, cancellationToken);
    }

    public async Task AddSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportScheduleCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.SupportSchedules.AddAsync(supportSchedule, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

    }

    public async Task UpdateSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportScheduleUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.SupportSchedules.Update(supportSchedule);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportScheduleDeletedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.SupportSchedules.Update(supportSchedule);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportScheduleRestoredV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.SupportSchedules.Update(supportSchedule);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}