using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeGroupDataStore : IGroupDataStore
{
    private readonly Group? _groupReturn;

    public FakeGroupDataStore(Group? groupReturn = null)
    {
        _groupReturn = groupReturn;
    }

    public bool GroupExistsResult { get; set; }
    public bool GroupAdded { get; private set; }
    public bool GroupUpdated { get; private set; }
    public bool GroupActivated { get; private set; }
    public bool GroupDeactivated { get; private set; }

    public Task<Group?> GetByIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken) => Task.FromResult(_groupReturn);
    public Task<bool> GroupExistsAsync(Guid tenantId, Guid educationalProgramId, string shift, string groupName, CancellationToken cancellationToken) => Task.FromResult(GroupExistsResult);
    public Task AddGroupWithOutboxAsync(Group group, GroupCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        GroupAdded = true;
        return Task.CompletedTask;
    }
    public Task UpdateGroupWithOutboxAsync(Group group, GroupUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        GroupUpdated = true;
        return Task.CompletedTask;
    }
    public Task ActivateGroupWithOutboxAsync(Group group, GroupActivateIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        GroupActivated = true;
        return Task.CompletedTask;
    }
    public Task DeactivateGroupWithOutboxAsync(Group group, GroupDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        GroupDeactivated = true;
        return Task.CompletedTask;
    }
}