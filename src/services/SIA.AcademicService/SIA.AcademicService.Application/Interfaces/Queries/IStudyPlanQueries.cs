using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.Queries;

public interface IStudyPlanQueries
{
    Task<StudyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<StudyPlan>> GetAllAsync(CancellationToken cancellationToken);
}
