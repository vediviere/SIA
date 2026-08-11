using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface IProfessorDataStore
{
    Task<bool> PersonAlreadyProfessorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task AddProfessorWithOutboxAsync(Professor professor, ProfessorCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<Professor?> GetProfessorByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken);

    Task UpdateProfessorWithOutboxAsync(Professor professor, ProfessorUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task ActivateProfessorWithOutboxAsync(Professor professor, ProfessorActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateProfessorWithOutboxAsync(Professor professor, ProfessorDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}