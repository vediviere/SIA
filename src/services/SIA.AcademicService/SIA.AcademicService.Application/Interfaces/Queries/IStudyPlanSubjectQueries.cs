using SIA.AcademicService.Application.DTOs.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public interface IStudyPlanSubjectQueries

    {
        Task<StudyPlanSubject?> GetByIdAsync(Guid tenantId, Guid studyPlanSubjectId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<StudyPlanSubject>> SearchAsync(Guid tenantId, StudyPlanSubjectFilter filter, CancellationToken cancellationToken);
    }
}