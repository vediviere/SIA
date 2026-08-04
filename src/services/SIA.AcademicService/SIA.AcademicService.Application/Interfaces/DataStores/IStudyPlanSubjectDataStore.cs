using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface IStudyPlanSubjectDataStore
{
    Task<bool> StudyPlanSubjectExistsAsync(Guid tenantId, Guid studyPlanId, Guid subjectId, CancellationToken cancellationToken);

    Task<StudyPlanSubject?> GetStudyPlanSubjectByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);

    Task AddStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task UpdateStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task SoftDeleteStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}