using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionHeads;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionHeads;

public sealed class DeactivateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public DeactivateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
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

        divisionHead.Deactivate();

        var integrationEvent = new DivisionHeadDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionHead.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = divisionHead.TenantId,
            DivisionHeadId = divisionHead.Id,
            Version = 1
        };

        await _dataStore.DeactivateDivisionHeadWithOutboxAsync(divisionHead, integrationEvent, cancellationToken);
    }
}