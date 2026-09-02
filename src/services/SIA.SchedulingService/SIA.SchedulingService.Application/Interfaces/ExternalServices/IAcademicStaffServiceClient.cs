namespace SIA.SchedulingService.Application.Interfaces.ExternalServices;

public interface IAcademicStaffServiceClient
{
    Task<IReadOnlyList<CandidateTeacherDto>> GetCandidateTeachersAsync(
        Guid tenantId,
        CancellationToken cancellationToken);
}