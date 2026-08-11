using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Contracts.Requests.Professors;
using SIA.AcademicStaffService.Contracts.Responses.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.Professors;

public sealed class CreateProfessorUseCase
{
    private readonly IProfessorDataStore _dataStore;

    public CreateProfessorUseCase(IProfessorDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateProfessorResponse> ExecuteAsync(
        CreateProfessorRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var personAlreadyProfessor = await _dataStore.PersonAlreadyProfessorAsync(
            request.TenantId,
            request.PersonId,
            cancellationToken);

        if (personAlreadyProfessor)
        {
            throw new DuplicateProfessorException(request.PersonId);
        }

        var professor = new Professor(
            request.TenantId,
            request.PersonId,
            request.AcademicDegree,
            request.ProfessionalProfile,
            request.ContractType,
            request.ContractHours);

        var integrationEvent = new ProfessorCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = professor.CreatedAtUtc,
            TenantId = professor.TenantId,
            ProfessorId = professor.Id,
            PersonId = professor.PersonId,
            AcademicDegree = professor.AcademicDegree,
            ProfessionalProfile = professor.ProfessionalProfile,
            ContractType = professor.ContractType,
            ContractHours = professor.ContractHours,
            Status = professor.Status,
            Version = 1
        };

        await _dataStore.AddProfessorWithOutboxAsync(professor, integrationEvent, cancellationToken);

        return new CreateProfessorResponse
        {
            Id = professor.Id,
            TenantId = professor.TenantId,
            PersonId = professor.PersonId,
            AcademicDegree = professor.AcademicDegree,
            ProfessionalProfile = professor.ProfessionalProfile,
            ContractType = professor.ContractType,
            ContractHours = professor.ContractHours,
            Status = professor.Status,
            CreatedAtUtc = professor.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}