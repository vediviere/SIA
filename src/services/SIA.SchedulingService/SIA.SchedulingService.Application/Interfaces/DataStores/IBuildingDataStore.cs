
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IBuildingDataStore
{
    Task<bool> BuildingCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);
    Task AddBuildingWithOutboxAsync(Building building, BuildingCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task<Building?> GetByIdAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken);
    Task UpdateBuildingWithOutboxAsync(Building building, BuildingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task DeactivateBuildingWithOutboxAsync(Building building, BuildingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task ActivateBuildingWithOutboxAsync(Building building, BuildingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
