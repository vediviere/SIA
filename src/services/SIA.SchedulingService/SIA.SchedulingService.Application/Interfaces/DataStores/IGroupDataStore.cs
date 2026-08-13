using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IGroupDataStore
{
    Task<bool> GroupExistsAsync(Guid tenantId, Guid programId, string shift, string name, CancellationToken cancellationToken);
    Task AddGroupWithOutboxAsync(Group group, GroupCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task<Group?> GetByIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);
    Task UpdateGroupWithOutboxAsync(Group group, GroupUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task DeactivateGroupWithOutboxAsync(Group group, GroupDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task ActivateGroupWithOutboxAsync(Group group, GroupActivateIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}