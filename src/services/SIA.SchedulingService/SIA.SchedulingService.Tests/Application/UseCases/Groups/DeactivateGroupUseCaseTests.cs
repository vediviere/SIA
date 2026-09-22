using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Groups;

public sealed class DeactivateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidGroup_ShouldDeactivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var group = new Group(tenantId, Guid.NewGuid(), "GRUPO A-ISIC", "MATUTINO", 9);

        var dataStore = new FakeGroupDataStore(group);
        var useCase = new DeactivateGroupUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, group.Id, correlationId, CancellationToken.None);

        Assert.False(group.Status);
        Assert.NotNull(group.UpdatedAtUtc);

        Assert.NotNull(dataStore.AddedDeactivatedEvent);
        Assert.Equal(correlationId, dataStore.AddedDeactivatedEvent.CorrelationId);
        Assert.False(dataStore.AddedDeactivatedEvent.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTenantIdDoesNotMatchGroup_ShouldThrowGroupNotFoundException()
    {
        var groupTenantId = Guid.NewGuid();
        var differentTenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var group = new Group(
            groupTenantId,
            Guid.NewGuid(),
            "GRUPO A-ISIC",
            "MATUTINO",
            30);

        var dataStore = new FakeGroupDataStore(group);
        var useCase = new DeactivateGroupUseCase(dataStore);

        await Assert.ThrowsAsync<GroupNotFoundException>(() =>
            useCase.ExecuteAsync(
                differentTenantId,
                group.Id,
                correlationId,
                CancellationToken.None));

        Assert.True(group.Status);
        Assert.Null(dataStore.AddedDeactivatedEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGroupDoesNotExist_ShouldThrowGroupNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeGroupDataStore(null);
        var useCase = new DeactivateGroupUseCase(dataStore);

        await Assert.ThrowsAsync<GroupNotFoundException>(() => useCase.ExecuteAsync(tenantId, groupId, correlationId, CancellationToken.None));

        Assert.Null(dataStore.AddedDeactivatedEvent);
    }
}