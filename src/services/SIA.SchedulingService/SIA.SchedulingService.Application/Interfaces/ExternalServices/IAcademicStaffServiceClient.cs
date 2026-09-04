namespace SIA.SchedulingService.Application.Interfaces.ExternalServices;

public interface IAcademicStaffServiceClient
{
    Task<IReadOnlyList<CandidateTeacherDto>> GetCandidateTeachersAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<CandidateTeacherDto?> GetTeacherAsync(Guid tenantId, Guid teacherId, CancellationToken cancellationToken);
}