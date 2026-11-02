using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;
using SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadGroups;

public sealed class AssignTutorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateAssignment()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var group = new Group(tenantId, Guid.NewGuid(), "Matutino", "A", 30);
        var academicLoad = new AcademicLoad(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "OF-2026", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
        var dataStore = new FakeAcademicLoadGroupDataStore(existsReturn: false);
        var groupDataStore = new FakeGroupDataStore(group);
        var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
        var useCase = new AssignTutorUseCase(dataStore, groupDataStore, academicLoadDataStore);
        var request = new AssignTutorRequest { GroupId = group.Id, AcademicLoadId = academicLoad.Id };

        var response = await useCase.ExecuteAsync(tenantId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(group.Id, response.GroupId);
        Assert.Equal(academicLoad.Id, response.AcademicLoadId);
        Assert.Equal(2, response.Hours);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedAssignment);
        Assert.Equal(group.Id, dataStore.AddedAssignment.GroupId);
        Assert.NotNull(dataStore.AddedAssignedEvent);
        Assert.Equal(correlationId, dataStore.AddedAssignedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGroupHasTutor_ShouldThrowGroupAlreadyHasTutorException()
    {
        var tenantId = Guid.NewGuid();
        var group = new Group(tenantId, Guid.NewGuid(), "Matutino", "A", 30);
        var academicLoad = new AcademicLoad(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "OF-2026", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
        var dataStore = new FakeAcademicLoadGroupDataStore(existsReturn: true);
        var groupDataStore = new FakeGroupDataStore(group);
        var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
        var useCase = new AssignTutorUseCase(dataStore, groupDataStore, academicLoadDataStore);
        var request = new AssignTutorRequest { GroupId = group.Id, AcademicLoadId = academicLoad.Id };

        await Assert.ThrowsAsync<GroupAlreadyHasTutorException>(() =>
            useCase.ExecuteAsync(tenantId, request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedAssignment);
        Assert.Null(dataStore.AddedAssignedEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGroupDoesNotExist_ShouldThrowGroupNotEligibleForTutorException()
    {
        var tenantId = Guid.NewGuid();
        var dataStore = new FakeAcademicLoadGroupDataStore();
        var groupDataStore = new FakeGroupDataStore(null);
        var academicLoadDataStore = new FakeAcademicLoadDataStore();
        var useCase = new AssignTutorUseCase(dataStore, groupDataStore, academicLoadDataStore);
        var request = new AssignTutorRequest { GroupId = Guid.NewGuid(), AcademicLoadId = Guid.NewGuid() };

        await Assert.ThrowsAsync<GroupNotEligibleForTutorException>(() =>
            useCase.ExecuteAsync(tenantId, request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedAssignment);
        Assert.Null(dataStore.AddedAssignedEvent);
    }
}