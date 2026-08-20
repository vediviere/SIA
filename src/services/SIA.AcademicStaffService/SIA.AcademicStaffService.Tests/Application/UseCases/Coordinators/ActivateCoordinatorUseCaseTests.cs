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

        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = coordinator };
        var useCase = new ActivateCoordinatorUseCase(dataStore);

        await useCase.ExecuteAsync(coordinator.TenantId, coordinator.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.True(coordinator.Status);
        Assert.True(dataStore.CoordinatorActivated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCoordinatorNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeCoordinatorDataStore { CoordinatorById = null };
        var useCase = new ActivateCoordinatorUseCase(dataStore);

        await Assert.ThrowsAsync<CoordinatorNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.CoordinatorActivated);
    }
}