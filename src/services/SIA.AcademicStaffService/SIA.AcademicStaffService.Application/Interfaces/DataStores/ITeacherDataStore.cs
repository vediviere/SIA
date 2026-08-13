using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface ITeacherDataStore
{
    Task<bool> PersonAlreadyProfessorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task AddProfessorWithOutboxAsync(Teacher professor, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<Teacher?> GetProfessorByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken);

    Task UpdateProfessorWithOutboxAsync(Teacher professor, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task ActivateProfessorWithOutboxAsync(Teacher professor, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateProfessorWithOutboxAsync(Teacher professor, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}