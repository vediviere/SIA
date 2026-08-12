using SIA.AcademicStaffService.Application.DTOs.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface ITeacherQueries
{
    Task<Teacher?> GetByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Teacher>> SearchAsync(TeacherFilter filter, CancellationToken cancellationToken);
}