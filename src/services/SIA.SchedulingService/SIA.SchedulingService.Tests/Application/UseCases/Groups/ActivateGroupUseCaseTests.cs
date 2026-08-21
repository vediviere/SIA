using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Groups;

public sealed class ActivateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidGroup_ShouldActivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var group = new Group(tenantId, Guid.NewGuid(), "GRUPO A-ISIC", "MATUTINO", 30);
        group.Deactivate();

        var dataStore = new FakeGroupDataStore(group);
        var useCase = new ActivateGroupUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, groupId, correlationId, CancellationToken.None);

        Assert.True(group.Status);
        Assert.NotNull(group.UpdatedAtUtc);

        Assert.NotNull(dataStore.AddedActivatedEvent);
        Assert.Equal(correlationId, dataStore.AddedActivatedEvent.CorrelationId);
        Assert.True(dataStore.AddedActivatedEvent.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGroupDoesNotExist_ShouldThrowGroupNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeGroupDataStore(null);
        var useCase = new ActivateGroupUseCase(dataStore);

        await Assert.ThrowsAsync<GroupNotFoundException>(() => useCase.ExecuteAsync(tenantId, groupId, correlationId, CancellationToken.None));
        Assert.Null(dataStore.AddedActivatedEvent);
    }
}