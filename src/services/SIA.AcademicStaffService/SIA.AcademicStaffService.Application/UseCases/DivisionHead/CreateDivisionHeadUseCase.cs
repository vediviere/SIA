using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Requests.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Responses.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionManagers;

public sealed class CreateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public CreateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateDivisionHeadResponse> ExecuteAsync(
        CreateDivisionHeadRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var personAlreadyManagesProgram = await _dataStore.PersonAlreadyManagesProgramAsync(
            request.TenantId,
            request.ProgramId,
            request.PersonId,
            cancellationToken);

        if (personAlreadyManagesProgram)
        {
            throw new DuplicateDivisionHeadException(request.ProgramId, request.PersonId);
        }

        var divisionManager = new DivisionHead(
            request.TenantId,
            request.ProgramId,
            request.PersonId);

        var integrationEvent = new DivisionHeadCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionManager.CreatedAtUtc,
            TenantId = divisionManager.TenantId,
            DivisionManagerId = divisionManager.Id,
            ProgramId = divisionManager.ProgramId,
            PersonId = divisionManager.PersonId,
            Status = divisionManager.Status,
            Version = 1
        };

        await _dataStore.AddDivisionManagerWithOutboxAsync(divisionManager, integrationEvent, cancellationToken);

        return new CreateDivisionHeadResponse
        {
            Id = divisionManager.Id,
            TenantId = divisionManager.TenantId,
            ProgramId = divisionManager.ProgramId,
            PersonId = divisionManager.PersonId,
            Status = divisionManager.Status,
            CreatedAtUtc = divisionManager.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}