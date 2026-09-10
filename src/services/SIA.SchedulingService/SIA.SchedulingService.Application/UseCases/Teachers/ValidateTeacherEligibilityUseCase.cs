using SIA.SchedulingService.Application.Interfaces.ExternalServices;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Contracts.Enums;
using SIA.SchedulingService.Contracts.Requests.Teachers;
using SIA.SchedulingService.Contracts.Responses.Teachers;

namespace SIA.SchedulingService.Application.UseCases.Teachers;

public sealed class ValidateTeacherEligibilityUseCase
{
    private readonly IAcademicStaffServiceClient _academicStaffServiceClient;
    private readonly IAcademicOfferingQueries _academicOfferingQueries;
    private readonly IGroupQueries _groupQueries;

    public ValidateTeacherEligibilityUseCase(
        IAcademicStaffServiceClient academicStaffServiceClient,
        IAcademicOfferingQueries academicOfferingQueries,
        IGroupQueries groupQueries)
    {
        _academicStaffServiceClient = academicStaffServiceClient;
        _academicOfferingQueries = academicOfferingQueries;
        _groupQueries = groupQueries;
    }

    public async Task<ValidateTeacherEligibilityResponse> ExecuteAsync(
        ValidateTeacherEligibilityRequest request,
        CancellationToken cancellationToken)
    {
        var reasons = new List<EligibilityRejectionReason>();

        var teacher = await _academicStaffServiceClient.GetTeacherAsync(
            request.TenantId, request.TeacherId, cancellationToken);

        if (teacher is null || !teacher.Status)
        {
            reasons.Add(EligibilityRejectionReason.TeacherNotAvailable);

            return new ValidateTeacherEligibilityResponse
            {
                Eligible = false,
                Reasons = reasons
            };
        }

        var offering = await _academicOfferingQueries.GetByIdAsync(
            request.TenantId, request.AcademicOfferingId, cancellationToken);

        if (offering is not null)
        {
            var assignedHours = await _academicOfferingQueries.GetAssignedClassHoursAsync(
                request.TenantId,
                request.TeacherId,
                request.AcademicPeriodId,
                request.AcademicOfferingId,
                cancellationToken);

            var availableHours = teacher.ContractHours - assignedHours;

            if (availableHours < offering.ClassHours)
            {
                reasons.Add(EligibilityRejectionReason.InsufficientAvailableHours);
            }
        }

        var group = await _groupQueries.GetByIdAsync(
            request.TenantId, request.GroupId, cancellationToken);

        if (group is not null && teacher.ProgramId.HasValue && teacher.ProgramId != group.EducationalProgramId)
        {
            reasons.Add(EligibilityRejectionReason.InvalidEducationalProgram);
        }

        return new ValidateTeacherEligibilityResponse
        {
            Eligible = reasons.Count == 0,
            Reasons = reasons
        };
    }
}