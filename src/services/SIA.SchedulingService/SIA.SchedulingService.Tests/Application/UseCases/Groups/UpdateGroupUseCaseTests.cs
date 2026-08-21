using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Contracts.Requests.Group;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Groups;

public sealed class UpdateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateGroup()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var existingGroup = new Group(tenantId, educationalProgramId, "GRUPO A-SISIC", "MATUTINO", 30);

        var dataStore = new FakeGroupDataStore(existingGroup);
        var useCase = new UpdateGroupUseCase(dataStore);

        var request = new UpdateGroupRequest
        {
            GroupName = "  grupo b  ",
            Shift = "  vespertino  ",
            Capacity = 35
        };
        var responseGroup = await useCase.ExecuteAsync(tenantId, groupId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, responseGroup.TenantId);
        Assert.Equal(educationalProgramId, responseGroup.EducationalProgramId);
        Assert.Equal("GRUPO B", responseGroup.GroupId);
        Assert.Equal("VESPERTINO", responseGroup.Shift);
        Assert.Equal(35, responseGroup.Capacity);
        Assert.NotNull(responseGroup.UpdatedAtUtc);
        Assert.Equal(correlationId, responseGroup.CorrelationId);

        Assert.NotNull(dataStore.UpdatedGroup);
        Assert.Equal("GRUPO B", dataStore.UpdatedGroup.GroupName);
        Assert.Equal("VESPERTINO", dataStore.UpdatedGroup.Shift);

        Assert.NotNull(dataStore.AddedUpdatedEvent);
        Assert.Equal(correlationId, dataStore.AddedUpdatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGroupDoesNotExist_ShouldThrowNotFound()
    {
        var dataStore = new FakeGroupDataStore(null);
        var useCase = new UpdateGroupUseCase(dataStore);

        var request = new UpdateGroupRequest
        {
            GroupName = "GRUPO A",
            Shift = "MATUTINO",
            Capacity = 30
        };
        await Assert.ThrowsAsync<GroupNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedGroup);
        Assert.Null(dataStore.AddedUpdatedEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNameOrShiftChangesAndAlreadyExists_ShouldThrowDuplicateGroupException()
    {
        var tenantId = Guid.NewGuid();
        var existingGroup = new Group(tenantId, Guid.NewGuid(), "GRUPO A", "MATUTINO", 30);

        var dataStore = new FakeGroupDataStore(existingGroup)
        {
            GroupExistsResult = true
        };
        var useCase = new UpdateGroupUseCase(dataStore);

        var request = new UpdateGroupRequest
        {
            GroupName = "GRUPO B",
            Shift = "MATUTINO",
            Capacity = 30
        };
        await Assert.ThrowsAsync<DuplicateGroupException>(() => useCase.ExecuteAsync(tenantId, Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedGroup);
        Assert.Null(dataStore.AddedUpdatedEvent);
    }
}