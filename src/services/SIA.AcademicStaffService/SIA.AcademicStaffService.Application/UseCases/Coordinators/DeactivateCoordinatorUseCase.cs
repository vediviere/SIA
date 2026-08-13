using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;

namespace SIA.AcademicStaffService.Application.UseCases.Coordinators;

public sealed class DeactivateCoordinatorUseCase
{
    private readonly ICoordinatorDataStore _dataStore;

    public DeactivateCoordinatorUseCase(ICoordinatorDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid coordinatorId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var coordinator = await _dataStore.GetCoordinatorByIdAsync(tenantId, coordinatorId, cancellationToken);

        if (coordinator is null)
        {
            throw new CoordinatorNotFoundException(coordinatorId);
        }

        coordinator.Deactivate();

        var integrationEvent = new CoordinatorDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = coordinator.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = coordinator.TenantId,
            CoordinatorId = coordinator.Id,
            Version = 1
        };

        await _dataStore.DeactivateCoordinatorWithOutboxAsync(coordinator, integrationEvent, cancellationToken);
    }
}