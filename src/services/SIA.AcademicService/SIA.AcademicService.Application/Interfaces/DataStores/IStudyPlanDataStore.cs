using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface IStudyPlanDataStore
{
    Task<bool> StudyPlanCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

    Task AddStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<StudyPlan?> GetByIdAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken);

    Task UpdateStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}