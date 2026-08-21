using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeBuildingDataStore : IBuildingDataStore
{
    private readonly Building? _buildingReturn;

    public FakeBuildingDataStore(Building? buildingReturn = null)
    {
        _buildingReturn = buildingReturn;
    }
    public bool CodeExistsResult { get; set; }
    public Building? AddedBuilding { get; private set; }
    public Building? UpdatedBuilding { get; private set; }
    public BuildingCreatedIntegrationEvent? AddedCreatedEvent { get; private set; }
    public BuildingUpdatedIntegrationEvent? AddedUpdatedEvent { get; private set; }
    public BuildingActivatedIntegrationEvent? AddedActivatedEvent { get; private set; }
    public BuildingDeactivatedIntegrationEvent? AddedDeactivatedEvent { get; private set; }

    public Task<Building?> GetByIdAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken) => Task.FromResult(_buildingReturn);
    public Task<bool> BuildingCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken) => Task.FromResult(CodeExistsResult);
    public Task AddBuildingWithOutboxAsync(Building building, BuildingCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedBuilding = building;
        AddedCreatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task UpdateBuildingWithOutboxAsync(Building building, BuildingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedBuilding = building;
        AddedUpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task ActivateBuildingWithOutboxAsync(Building building, BuildingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task DeactivateBuildingWithOutboxAsync(Building building, BuildingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedDeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}