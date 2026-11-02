using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadGroups;

public sealed class ReplaceTutorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldReplaceAssignment()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var oldAssignment = new AcademicLoadGroup(tenantId, Guid.NewGuid(), groupId);
        var newAcademicLoad = new AcademicLoad(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "OF-NEW", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
        var dataStore = new FakeAcademicLoadGroupDataStore(assignmentReturn: oldAssignment);
        var academicLoadDataStore = new FakeAcademicLoadDataStore(newAcademicLoad);
        var useCase = new ReplaceTutorUseCase(dataStore, academicLoadDataStore);
        var request = new ReplaceTutorRequest { GroupId = groupId, AcademicLoadId = newAcademicLoad.Id };

        var response = await useCase.ExecuteAsync(tenantId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(groupId, response.GroupId);
        Assert.Equal(newAcademicLoad.Id, response.AcademicLoadId);
        Assert.NotNull(dataStore.ReplacedOldAssignment);
        Assert.NotNull(dataStore.ReplacedNewAssignment);
        Assert.NotNull(dataStore.AddedRemovedEvent);
        Assert.NotNull(dataStore.AddedAssignedEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAcademicLoadDoesNotExist_ShouldThrowAcademicLoadNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var oldAssignment = new AcademicLoadGroup(tenantId, Guid.NewGuid(), groupId);
        var dataStore = new FakeAcademicLoadGroupDataStore(assignmentReturn: oldAssignment);
        var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoadReturn: null);
        var useCase = new ReplaceTutorUseCase(dataStore, academicLoadDataStore);
        var request = new ReplaceTutorRequest { GroupId = groupId, AcademicLoadId = Guid.NewGuid() };

        await Assert.ThrowsAsync<AcademicLoadNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId, request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.ReplacedOldAssignment);
        Assert.Null(dataStore.ReplacedNewAssignment);
        Assert.Null(dataStore.AddedRemovedEvent);
        Assert.Null(dataStore.AddedAssignedEvent);
    }
}