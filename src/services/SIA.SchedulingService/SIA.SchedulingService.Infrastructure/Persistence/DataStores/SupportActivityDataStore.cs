using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class SupportActivityDataStore : ISupportActivityDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public SupportActivityDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SupportActivity?> GetSupportActivityByIdAsync(Guid tenantId, Guid supportActivityId, CancellationToken cancellationToken)
    {
        return await _dbContext.SupportActivities
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == supportActivityId, cancellationToken);
    }

    public async Task AddSupportActivityWithOutboxAsync(
        SupportActivity supportActivity,
        SupportActivityCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportActivityCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.SupportActivities.AddAsync(supportActivity, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSupportActivityWithOutboxAsync(
        SupportActivity supportActivity,
        SupportActivityUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportActivityUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.SupportActivities.Update(supportActivity);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

    }

    public async Task SoftDeleteSupportActivityWithOutboxAsync(
        SupportActivity supportActivity,
        SupportActivityDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportActivityDeletedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.SupportActivities.Update(supportActivity);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreSupportActivityWithOutboxAsync(
        SupportActivity supportActivity,
        SupportActivityRestoredIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.SupportActivityRestoredV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.SupportActivities.Update(supportActivity);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}