using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface IStudyPlanDataStore
{
    Task<bool> StudyPlanCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

    Task AddStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<StudyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(StudyPlan studyPlan, CancellationToken cancellationToken);
}