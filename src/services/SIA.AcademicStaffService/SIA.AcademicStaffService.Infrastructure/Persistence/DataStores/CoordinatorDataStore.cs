using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.DataStores;

public sealed class CoordinatorDataStore : ICoordinatorDataStore
{
    private readonly AcademicStaffDbContext _dbContext;

    public CoordinatorDataStore(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> PersonAlreadyCoordinatorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
    {
        return _dbContext.Coordinators.AnyAsync(
            coordinator => coordinator.TenantId == tenantId && coordinator.PersonId == personId,
            cancellationToken);
    }

    public Task<Coordinator?> GetCoordinatorByIdAsync(Guid tenantId, Guid coordinatorId, CancellationToken cancellationToken)
    {
        return _dbContext.Coordinators.FirstOrDefaultAsync(
            coordinator => coordinator.TenantId == tenantId && coordinator.Id == coordinatorId,
            cancellationToken);
    }

    public async Task AddCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.CoordinatorCreatedV1, payload, integrationEvent.CorrelationId);

        await _dbContext.Coordinators.AddAsync(coordinator, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.CoordinatorActivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Coordinators.Update(coordinator);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.CoordinatorDeactivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Coordinators.Update(coordinator);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}