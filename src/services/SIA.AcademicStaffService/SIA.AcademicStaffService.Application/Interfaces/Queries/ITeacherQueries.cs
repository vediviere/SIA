using SIA.AcademicStaffService.Application.DTOs.Teacher;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface ITeacherQueries
{
    Task<TeacherDto?> GetByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<TeacherDto>> SearchAsync(TeacherFilter filter, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<TeacherCandidateDto>> GetCandidatesAsync(CandidateTeacherFilter filter, CancellationToken cancellationToken);
}