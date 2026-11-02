using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Contracts.IntegrationEvents;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class AcademicLoadGroupDataStore : IAcademicLoadGroupDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public AcademicLoadGroupDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicLoadGroups
            .AnyAsync(x => x.TenantId == tenantId && x.GroupId == groupId, cancellationToken);
    }

    public Task<AcademicLoadGroup?> GetByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicLoadGroups
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.GroupId == groupId, cancellationToken);
    }

    public async Task AddAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup academicLoadGroup,
        TutorAssignedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = "TutorAssignedIntegrationEventV1";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.AcademicLoadGroups.AddAsync(academicLoadGroup, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup academicLoadGroup,
        TutorRemovedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = "TutorRemovedIntegrationEventV1";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.AcademicLoadGroups.Remove(academicLoadGroup);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup oldAssignment, AcademicLoadGroup newAssignment,
        TutorRemovedIntegrationEvent removedEvent, TutorAssignedIntegrationEvent assignedEvent, CancellationToken cancellationToken)
    {
        var removedPayload = JsonSerializer.Serialize(removedEvent);
        var removedOutboxMessage = new OutboxMessage("TutorRemovedIntegrationEventV1", removedPayload, removedEvent.CorrelationId);
        var assignedPayload = JsonSerializer.Serialize(assignedEvent);
        var assignedOutboxMessage = new OutboxMessage("TutorAssignedIntegrationEventV1", assignedPayload, assignedEvent.CorrelationId);

        _dbContext.AcademicLoadGroups.Remove(oldAssignment);
        await _dbContext.AcademicLoadGroups.AddAsync(newAssignment, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(removedOutboxMessage, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(assignedOutboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}