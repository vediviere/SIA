using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;
public interface IEducationalProgramsDataStore
{
    Task<bool> EducationalProgramCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

    Task AddEducationalProgramWithOutboxAsync(EducationalPrograms educationalProgram, EducationalProgramCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<EducationalPrograms?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task Update(EducationalPrograms educationalPrograms, CancellationToken cancellationToken);
}
