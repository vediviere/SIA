using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface ISubjectDataStore
{
  Task<bool> SubjectCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

  Task AddSubjectWithOutboxAsync(Subject subject, SubjectCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<Subject?> GetSubjectByIdAsync(Guid tenantId, Guid subjectId, CancellationToken cancellationToken);

    Task UpdateSubjectWithOutboxAsync(Subject subject, SubjectUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task SoftDeleteSubjectWithOutboxAsync(Subject subject, SubjectDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreSubjectWithOutboxAsync(Subject subject, SubjectRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
