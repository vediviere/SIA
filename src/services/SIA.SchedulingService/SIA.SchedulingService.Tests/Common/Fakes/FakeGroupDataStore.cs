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
    public Group? AddedGroup { get; private set; }
    public Group? UpdatedGroup { get; private set; }
    public GroupCreatedIntegrationEvent? AddedCreatedEvent { get; private set; }
    public GroupUpdatedIntegrationEvent? AddedUpdatedEvent { get; private set; }
    public GroupActivateIntegrationEvent? AddedActivatedEvent { get; private set; }
    public GroupDeactivatedIntegrationEvent? AddedDeactivatedEvent { get; private set; }

    public Task<Group?> GetByIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken) => Task.FromResult(_groupReturn);
    public Task<bool> GroupExistsAsync(Guid tenantId, Guid educationalProgramId, string shift, string groupName, CancellationToken cancellationToken) => Task.FromResult(GroupExistsResult);
    public Task AddGroupWithOutboxAsync(Group group, GroupCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedGroup = group;
        AddedCreatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task UpdateGroupWithOutboxAsync(Group group, GroupUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedGroup = group;
        AddedUpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task ActivateGroupWithOutboxAsync(Group group, GroupActivateIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task DeactivateGroupWithOutboxAsync(Group group, GroupDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedDeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}