using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Teacher;
using SIA.AcademicStaffService.Contracts.Requests.Teacher;
using SIA.AcademicStaffService.Contracts.Responses.Teacher;

namespace SIA.AcademicStaffService.Application.UseCases.Teachers;

public sealed class UpdateTeacherUseCase
{
    private readonly ITeacherDataStore _dataStore;

    public UpdateTeacherUseCase(ITeacherDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateTeacherResponse> ExecuteAsync(
        Guid tenantId,
        Guid teacherId,
        UpdateTeacherRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var teacher = await _dataStore.GetTeacherByIdAsync(tenantId, teacherId, cancellationToken);

        if (teacher is null)
        {
            throw new TeacherNotFoundException(teacherId);
        }

        teacher.Update(
            request.ProfessionalProfile,
            request.ContractType,
            request.ContractHours);

        var integrationEvent = new TeacherUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = teacher.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = teacher.TenantId,
            TeacherId = teacher.Id,
            PersonId = teacher.PersonId,
            ProfessionalProfile = teacher.ProfessionalProfile,
            ContractType = teacher.ContractType,
            ContractHours = teacher.ContractHours,
            Status = teacher.Status,
            Version = 1
        };

        await _dataStore.UpdateTeacherWithOutboxAsync(teacher, integrationEvent, cancellationToken);

        return new UpdateTeacherResponse
        {
            Id = teacher.Id,
            TenantId = teacher.TenantId,
            PersonId = teacher.PersonId,
            ProfessionalProfile = teacher.ProfessionalProfile,
            ContractType = teacher.ContractType,
            ContractHours = teacher.ContractHours,
            Status = teacher.Status,
            UpdatedAtUtc = teacher.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}