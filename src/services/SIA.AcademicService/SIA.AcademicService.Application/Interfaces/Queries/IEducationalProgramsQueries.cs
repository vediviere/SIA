using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.Queries;

public interface IEducationalProgramsQueries
{
    Task<EducationalProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<EducationalProgram>> GetAllAsync(CancellationToken cancellationToken);
}
