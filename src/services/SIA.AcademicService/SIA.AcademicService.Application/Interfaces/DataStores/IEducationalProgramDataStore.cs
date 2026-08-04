using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;
public interface IEducationalProgramDataStore
{
    Task<bool> EducationalProgramCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

    Task AddEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<EducationalProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(EducationalProgram educationalPrograms, CancellationToken cancellationToken);
}
