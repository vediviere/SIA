using SIA.AcademicStaffService.Contracts.IntegrationEvents.Teacher;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface ITeacherDataStore
{
    Task<bool> PersonAlreadyTeacherAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task AddTeacherWithOutboxAsync(Teacher teacher, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<Teacher?> GetTeacherByIdAsync(Guid tenantId, Guid teacherId, CancellationToken cancellationToken);

    Task UpdateTeacherWithOutboxAsync(Teacher teacher, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task ActivateTeacherWithOutboxAsync(Teacher teacher, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateTeacherWithOutboxAsync(Teacher teacher, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}