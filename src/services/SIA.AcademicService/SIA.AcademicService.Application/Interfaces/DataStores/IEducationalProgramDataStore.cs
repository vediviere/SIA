using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;
public interface IEducationalProgramDataStore
{
    Task<bool> EducationalProgramCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

    Task AddEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<EducationalProgram?> GetByIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken);

    Task UpdateEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
