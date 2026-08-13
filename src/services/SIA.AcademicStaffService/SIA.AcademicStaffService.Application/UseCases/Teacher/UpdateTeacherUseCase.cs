using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Contracts.Requests.Professors;
using SIA.AcademicStaffService.Contracts.Responses.Professors;

namespace SIA.AcademicStaffService.Application.UseCases.Professors;

public sealed class UpdateTeacherUseCase
{
    private readonly ITeacherDataStore _dataStore;

    public UpdateTeacherUseCase(ITeacherDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateTeacherResponse> ExecuteAsync(
        Guid tenantId,
        Guid professorId,
        UpdateTeacherRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var professor = await _dataStore.GetProfessorByIdAsync(tenantId, professorId, cancellationToken);

        if (professor is null)
        {
            throw new TeacherNotFoundException(professorId);
        }

        professor.Update(
            request.AcademicDegree,
            request.ProfessionalProfile,
            request.ContractType,
            request.ContractHours);

        var integrationEvent = new TeacherUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = professor.UpdatedAtUtc ?? DateTime.UtcNow,
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

        await _dataStore.UpdateProfessorWithOutboxAsync(professor, integrationEvent, cancellationToken);

        return new UpdateTeacherResponse
        {
            Id = professor.Id,
            TenantId = professor.TenantId,
            PersonId = professor.PersonId,
            AcademicDegree = professor.AcademicDegree,
            ProfessionalProfile = professor.ProfessionalProfile,
            ContractType = professor.ContractType,
            ContractHours = professor.ContractHours,
            Status = professor.Status,
            UpdatedAtUtc = professor.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}