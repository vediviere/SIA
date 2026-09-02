using SIA.SchedulingService.Application.Interfaces.ExternalServices;

namespace SIA.SchedulingService.Application.UseCases.Teachers;

public sealed class GetCandidateTeachersUseCase
{
    private readonly IAcademicStaffServiceClient _academicStaffServiceClient;

    public GetCandidateTeachersUseCase(IAcademicStaffServiceClient academicStaffServiceClient)
    {
        _academicStaffServiceClient = academicStaffServiceClient;
    }

    public async Task<IReadOnlyList<CandidateTeacherDto>> ExecuteAsync(
        Guid tenantId,
        Guid? programId,
        CancellationToken cancellationToken)
    {
        var candidates = await _academicStaffServiceClient.GetCandidateTeachersAsync(tenantId, cancellationToken);
        return candidates
            .OrderBy(candidate => programId.HasValue && candidate.ProgramId == programId ? 0 : 1)
            .ThenBy(candidate => candidate.ProfessionalProfile)
            .ToList();
    }
}