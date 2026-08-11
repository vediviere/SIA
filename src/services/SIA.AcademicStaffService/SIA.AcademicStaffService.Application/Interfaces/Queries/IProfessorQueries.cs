using SIA.AcademicStaffService.Application.DTOs.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface IProfessorQueries
{
    Task<Professor?> GetByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Professor>> SearchAsync(ProfessorFilter filter, CancellationToken cancellationToken);
}