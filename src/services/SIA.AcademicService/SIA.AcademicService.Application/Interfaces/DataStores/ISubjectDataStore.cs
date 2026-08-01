using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface ISubjectDataStore
{
  Task<bool> SubjectCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

  Task AddSubjectWithOutboxAsync(Subject subject, SubjectCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
