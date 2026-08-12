
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.Buildings;

public sealed class ActivateBuildingUseCase
{
    private readonly IBuildingDataStore _dataStore;

    public ActivateBuildingUseCase(IBuildingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
    {
        var building = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if(building == null)
        {
            throw new BuildingNotFoundException(id);
        }

        building.Activate();

        var integrationEvent = new BuildingDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = building.UpdatedAtUtc!.Value,
            TenantId = building.TenantId,
            BuildingId = building.Id,
            Status = building.Status,
            Version = 1
        };

        await _dataStore.ActivateBuildingWithOutboxAsync(building, integrationEvent, cancellationToken);
    }
}
