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

        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = coordinator };
        var useCase = new DeactivateCoordinatorUseCase(dataStore);

        await useCase.ExecuteAsync(coordinator.TenantId, coordinator.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(coordinator.Status);
        Assert.True(dataStore.CoordinatorDeactivated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCoordinatorNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = null };
        var useCase = new DeactivateCoordinatorUseCase(dataStore);

        await Assert.ThrowsAsync<CoordinatorNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.CoordinatorDeactivated);
    }
}