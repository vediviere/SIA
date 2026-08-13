using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionManagers;

public sealed class DeactivateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public DeactivateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid divisionManagerId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var divisionManager = await _dataStore.GetDivisionManagerByIdAsync(tenantId, divisionManagerId, cancellationToken);

        if (divisionManager is null)
        {
            throw new DivisionHeadNotFoundException(divisionManagerId);
        }

        divisionManager.Deactivate();

        var integrationEvent = new DivisionHeadDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionManager.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = divisionManager.TenantId,
            DivisionManagerId = divisionManager.Id,
            Version = 1
        };

        await _dataStore.DeactivateDivisionManagerWithOutboxAsync(divisionManager, integrationEvent, cancellationToken);
    }
}