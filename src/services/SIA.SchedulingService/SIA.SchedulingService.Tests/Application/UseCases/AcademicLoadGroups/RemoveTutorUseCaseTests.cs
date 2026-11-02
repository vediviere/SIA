using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;
using SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadGroups;

public sealed class RemoveTutorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingAssignment_ShouldRemoveAssignment()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var assignment = new AcademicLoadGroup(tenantId, Guid.NewGuid(), Guid.NewGuid());
        var dataStore = new FakeAcademicLoadGroupDataStore(assignmentReturn: assignment);
        var useCase = new RemoveTutorUseCase(dataStore);
        var request = new RemoveTutorRequest { GroupId = assignment.GroupId };

        await useCase.ExecuteAsync(tenantId, request, correlationId, CancellationToken.None);

        Assert.NotNull(dataStore.RemovedAssignment);
        Assert.NotNull(dataStore.AddedRemovedEvent);
        Assert.Equal(assignment.GroupId, dataStore.RemovedAssignment.GroupId);
        Assert.Equal(correlationId, dataStore.AddedRemovedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAssignmentDoesNotExist_ShouldThrowTutorAssignmentNotFoundException()
    {
        var dataStore = new FakeAcademicLoadGroupDataStore(assignmentReturn: null);
        var useCase = new RemoveTutorUseCase(dataStore);
        var request = new RemoveTutorRequest { GroupId = Guid.NewGuid() };

        await Assert.ThrowsAsync<TutorAssignmentNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.RemovedAssignment);
        Assert.Null(dataStore.AddedRemovedEvent);
    }
}