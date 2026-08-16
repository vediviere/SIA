using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.BuildingBlocks.Messaging.Outbox;
using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class AcademicLoadDataStore : IAcademicLoadDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public AcademicLoadDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAcademicLoadWithOutboxAsync(AcademicLoad academicLoad, AcademicLoadCreatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.AcademicLoadCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.AcademicLoad.AddAsync(academicLoad, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public  Task<AcademicLoad?> GetByIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicLoad.FirstOrDefaultAsync(academicLoad => academicLoad.TenantId == tenantId && academicLoad.Id == academicLoadId, cancellationToken);
    }

    public async Task UpdateAcademicLoadWithOutboxAsync (AcademicLoad academicLoad, AcademicLoadUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.AcademicLoadUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
    }

    public async Task DeactivateAcademicLoadWithOutboxAsync(AcademicLoad academicLoad, AcademicLoadDeactivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.AcademicLoadDeactivatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAcademicLoadWithOutboxAsync(AcademicLoad academicLoad, AcademicLoadActivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.AcademicLoadActivatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}