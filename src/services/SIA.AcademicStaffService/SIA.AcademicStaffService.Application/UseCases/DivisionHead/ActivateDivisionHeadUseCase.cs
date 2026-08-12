using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionManagers;

public sealed class ActivateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public ActivateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
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

        divisionManager.Activate();

        var integrationEvent = new DivisionHeadActivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionManager.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = divisionManager.TenantId,
            DivisionManagerId = divisionManager.Id,
            Version = 1
        };

        await _dataStore.ActivateDivisionManagerWithOutboxAsync(divisionManager, integrationEvent, cancellationToken);
    }
}