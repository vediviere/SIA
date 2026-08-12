using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Requests.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Responses.DivisionManagers;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionManagers;

public sealed class UpdateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public UpdateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateDivisionHeadResponse> ExecuteAsync(
        Guid tenantId,
        Guid divisionManagerId,
        UpdateDivisionHeadRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var divisionManager = await _dataStore.GetDivisionManagerByIdAsync(tenantId, divisionManagerId, cancellationToken);

        if (divisionManager is null)
        {
            throw new DivisionHeadNotFoundException(divisionManagerId);
        }

        divisionManager.Update(request.AcademicDegree);

        var integrationEvent = new DivisionHeadUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionManager.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = divisionManager.TenantId,
            DivisionManagerId = divisionManager.Id,
            ProgramId = divisionManager.ProgramId,
            PersonId = divisionManager.PersonId,
            AcademicDegree = divisionManager.AcademicDegree,
            Status = divisionManager.Status,
            Version = 1
        };

        await _dataStore.UpdateDivisionManagerWithOutboxAsync(divisionManager, integrationEvent, cancellationToken);

        return new UpdateDivisionHeadResponse
        {
            Id = divisionManager.Id,
            TenantId = divisionManager.TenantId,
            ProgramId = divisionManager.ProgramId,
            PersonId = divisionManager.PersonId,
            AcademicDegree = divisionManager.AcademicDegree,
            Status = divisionManager.Status,
            UpdatedAtUtc = divisionManager.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}