using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Coordinators;

public sealed class DeactivateCoordinatorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingCoordinator_ShouldDeactivate()
    {
        var coordinator = new Coordinator(Guid.NewGuid(), Guid.NewGuid());
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = coordinator };
        var useCase = new DeactivateCoordinatorUseCase(dataStore);

        await useCase.ExecuteAsync(coordinator.TenantId, coordinator.Id, correlationId, CancellationToken.None);

        Assert.False(coordinator.Status);
        Assert.NotNull(dataStore.DeactivatedCoordinator);
        Assert.Equal(coordinator.Id, dataStore.DeactivatedCoordinator.Id);
        Assert.NotNull(dataStore.DeactivatedEvent);
        Assert.Equal(coordinator.Id, dataStore.DeactivatedEvent.CoordinatorId);
        Assert.Equal(coordinator.TenantId, dataStore.DeactivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.DeactivatedEvent.CorrelationId);

    }

    [Fact]
    public async Task ExecuteAsync_WhenCoordinatorNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = null };
        var useCase = new DeactivateCoordinatorUseCase(dataStore);

        await Assert.ThrowsAsync<CoordinatorNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeactivatedCoordinator);
        Assert.Null(dataStore.DeactivatedEvent);
    }
}