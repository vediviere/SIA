using System;
using System.Threading;
using System.Threading.Tasks;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores
{
    public interface ITeacherCompetencyDataStore
    {
        Task<bool> CompetencyAlreadyExistsForTeacherAsync(Guid tenantId, Guid teacherId, Guid competencyId, CancellationToken cancellationToken);
        Task<TeacherCompetency?> GetTeacherCompetencyByIdAsync(Guid tenantId, Guid teacherCompetencyId, CancellationToken cancellationToken);
        Task AddTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
        Task UpdateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
        Task ActivateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
        Task DeactivateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}