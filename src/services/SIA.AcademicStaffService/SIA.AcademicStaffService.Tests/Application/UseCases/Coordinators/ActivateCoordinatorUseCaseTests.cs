using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Coordinators;

public sealed class ActivateCoordinatorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingCoordinator_ShouldActivate()
    {
        var coordinator = new Coordinator(Guid.NewGuid(), Guid.NewGuid());
        coordinator.Deactivate();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = coordinator };
        var useCase = new ActivateCoordinatorUseCase(dataStore);

        await useCase.ExecuteAsync(coordinator.TenantId, coordinator.Id, correlationId, CancellationToken.None);

        Assert.True(coordinator.Status);
        Assert.NotNull(dataStore.ActivatedCoordinator);
        Assert.Equal(coordinator.Id, dataStore.ActivatedCoordinator.Id);
        Assert.NotNull(dataStore.ActivatedEvent);
        Assert.Equal(coordinator.Id, dataStore.ActivatedEvent.CoordinatorId);
        Assert.Equal(coordinator.TenantId, dataStore.ActivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.ActivatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCoordinatorNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = null };
        var useCase = new ActivateCoordinatorUseCase(dataStore);

        await Assert.ThrowsAsync<CoordinatorNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.ActivatedCoordinator);
        Assert.Null(dataStore.ActivatedEvent);
    }
}