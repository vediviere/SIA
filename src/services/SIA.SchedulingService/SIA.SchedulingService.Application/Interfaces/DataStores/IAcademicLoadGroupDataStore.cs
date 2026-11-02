using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IAcademicLoadGroupDataStore
{
    Task<bool> ExistsByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);
    Task AddAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup academicLoadGroup, TutorAssignedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);
    Task<AcademicLoadGroup?> GetByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);
    Task RemoveAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup academicLoadGroup, TutorRemovedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);
    Task ReplaceAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup oldAssignment,
        AcademicLoadGroup newAssignment,
        TutorRemovedIntegrationEvent removedEvent, TutorAssignedIntegrationEvent assignedEvent, CancellationToken cancellationToken);
}