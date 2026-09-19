using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionHeads;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionHeads;

public sealed class ActivateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public ActivateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid divisionHeadId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var divisionHead = await _dataStore.GetDivisionHeadByIdAsync(tenantId, divisionHeadId, cancellationToken);

        if (divisionHead is null)
        {
            throw new DivisionHeadNotFoundException(divisionHeadId);
        }

        divisionHead.Activate();

        var integrationEvent = new DivisionHeadActivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionHead.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = divisionHead.TenantId,
            DivisionHeadId = divisionHead.Id,
            Version = 1
        };

        await _dataStore.ActivateDivisionHeadWithOutboxAsync(divisionHead, integrationEvent, cancellationToken);
    }
}