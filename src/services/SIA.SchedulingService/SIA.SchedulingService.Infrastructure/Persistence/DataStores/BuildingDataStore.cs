using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class BuildingDataStore : IBuildingDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public BuildingDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> BuildingCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return _dbContext.Buildings.AnyAsync(building => building.TenantId == tenantId && building.Code == code, cancellationToken);
    }

    public async Task AddBuildingWithOutboxAsync(Building building, BuildingCreatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(BuildingCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.Buildings.AddAsync(building, cancellationToken);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<Building?> GetByIdAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken)
    {
        return _dbContext.Buildings.FirstOrDefaultAsync(building => building.TenantId == tenantId && building.Id == buildingId, cancellationToken);
    }

    public async Task UpdateBuildingWithOutboxAsync(Building building, BuildingUpdatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(BuildingUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeactivateBuildingWithOutboxAsync(Building building, BuildingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(BuildingDeactivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType , payload, integrationEvent.CorrelationId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task ActivateBuildingWithOutboxAsync(Building building, BuildingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(BuildingActivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }


}
