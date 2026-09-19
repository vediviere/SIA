using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionHeads;
using SIA.AcademicStaffService.Contracts.Requests.DivisionHeads;
using SIA.AcademicStaffService.Contracts.Responses.DivisionHeads;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.DivisionHeads;

public sealed class CreateDivisionHeadUseCase
{
    private readonly IDivisionHeadDataStore _dataStore;

    public CreateDivisionHeadUseCase(IDivisionHeadDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateDivisionHeadResponse> ExecuteAsync(
        Guid tenantId,
        CreateDivisionHeadRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var personAlreadyManagesProgram = await _dataStore.PersonAlreadyManagesProgramAsync(
            tenantId,
            request.ProgramId,
            request.PersonId,
            cancellationToken);

        if (personAlreadyManagesProgram)
        {
            throw new DuplicateDivisionHeadException(request.ProgramId, request.PersonId);
        }

        var divisionHead = new DivisionHead(
            tenantId,
            request.ProgramId,
            request.PersonId);

        var integrationEvent = new DivisionHeadCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = divisionHead.CreatedAtUtc,
            TenantId = divisionHead.TenantId,
            DivisionHeadId = divisionHead.Id,
            ProgramId = divisionHead.ProgramId,
            PersonId = divisionHead.PersonId,
            Status = divisionHead.Status,
            Version = 1
        };

        await _dataStore.AddDivisionHeadWithOutboxAsync(divisionHead, integrationEvent, cancellationToken);

        return new CreateDivisionHeadResponse
        {
            Id = divisionHead.Id,
            TenantId = divisionHead.TenantId,
            ProgramId = divisionHead.ProgramId,
            PersonId = divisionHead.PersonId,
            Status = divisionHead.Status,
            CreatedAtUtc = divisionHead.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}