using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Contracts.Requests.Group;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Groups;

public sealed class CreateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidData_ShouldCreateGroup()
    {
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeGroupDataStore();
        var useCase = new CreateGroupUseCase(dataStore);

        var request = new CreateGroupRequest
        {
            TenantId = tenantId,
            EducationalProgramId = educationalProgramId,
            GroupName = "  grupo a-isic  ",
            Shift = "  vespertino  ",
            Capacity = 9
        };
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(educationalProgramId, response.EducationalProgramId);
        Assert.Equal("GRUPO A-ISIC", response.GroupName);
        Assert.Equal("VESPERTINO", response.Shift);
        Assert.Equal(9, response.Capacity);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.GroupAdded);
    }

    [Fact]
    public async Task ExecuteAsync_GroupAlreadyExists_ThrowConflict()
    {
        var dataStore = new FakeGroupDataStore { GroupExistsResult = true };
        var useCase = new CreateGroupUseCase(dataStore);

        var request = new CreateGroupRequest
        {
            TenantId = Guid.NewGuid(),
            EducationalProgramId = Guid.NewGuid(),
            GroupName = "Grupo A-ISIC",
            Shift = "Vespertino",
            Capacity = 9
        };
        await Assert.ThrowsAsync<DuplicateGroupException>(() => useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));
    }
}