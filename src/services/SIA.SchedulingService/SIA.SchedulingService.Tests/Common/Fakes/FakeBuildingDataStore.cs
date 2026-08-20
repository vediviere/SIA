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
    public bool BuildingAdd { get; private set; }
    public bool BuildingUpdated { get; private set; }
    public bool BuildingActivated { get; private set; }
    public bool BuildingDeactivated { get; private set; }

    public Task<Building?> GetByIdAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken) => Task.FromResult(_buildingReturn);
    public Task<bool> BuildingCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken) => Task.FromResult(CodeExistsResult);
    public Task AddBuildingWithOutboxAsync(Building building, BuildingCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        BuildingAdd = true;
        return Task.CompletedTask;
    }
    public Task UpdateBuildingWithOutboxAsync(Building building, BuildingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        BuildingUpdated = true;
        return Task.CompletedTask;
    }
    public Task ActivateBuildingWithOutboxAsync(Building building, BuildingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        BuildingActivated = true;
        return Task.CompletedTask;
    }
    public Task DeactivateBuildingWithOutboxAsync(Building building, BuildingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        BuildingDeactivated = true;
        return Task.CompletedTask;
    }
}