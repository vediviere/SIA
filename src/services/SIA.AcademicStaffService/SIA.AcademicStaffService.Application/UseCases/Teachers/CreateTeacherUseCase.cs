using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Teacher;
using SIA.AcademicStaffService.Contracts.Requests.Teacher;
using SIA.AcademicStaffService.Contracts.Responses.Teacher;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.Teachers;

public sealed class CreateTeacherUseCase
{
    private readonly ITeacherDataStore _dataStore;

    public CreateTeacherUseCase(ITeacherDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateTeacherResponse> ExecuteAsync(
        Guid tenantId,
        CreateTeacherRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var personAlreadyTeacher = await _dataStore.PersonAlreadyTeacherAsync(
            tenantId,
            request.PersonId,
            cancellationToken);

        if (personAlreadyTeacher)
        {
            throw new DuplicateTeacherException(request.PersonId);
        }

        var teacher = new Teacher(
            tenantId,
            request.PersonId,
            request.ProfessionalProfile,
            request.ContractType,
            request.ContractHours);

        var integrationEvent = new TeacherCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = teacher.CreatedAtUtc,
            TenantId = teacher.TenantId,
            TeacherId = teacher.Id,
            PersonId = teacher.PersonId,
            ProfessionalProfile = teacher.ProfessionalProfile,
            ContractType = teacher.ContractType,
            ContractHours = teacher.ContractHours,
            Status = teacher.Status,
            Version = 1
        };

        await _dataStore.AddTeacherWithOutboxAsync(teacher, integrationEvent, cancellationToken);

        return new CreateTeacherResponse
        {
            Id = teacher.Id,
            TenantId = teacher.TenantId,
            PersonId = teacher.PersonId,
            ProfessionalProfile = teacher.ProfessionalProfile,
            ContractType = teacher.ContractType,
            ContractHours = teacher.ContractHours,
            Status = teacher.Status,
            CreatedAtUtc = teacher.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}