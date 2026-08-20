using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Buildings;

public sealed class DeactivateBuildingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidBuilding_DeactivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var building = new Building(tenantId, "A1", "Edificio A", "EDIFICIO A-ISIC");

        var dataStore = new FakeBuildingDataStore(building);
        var useCase = new DeactivateBuildingUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, buildingId, correlationId, CancellationToken.None);

        Assert.False(building.Status);
        Assert.NotNull(building.UpdatedAtUtc);
        Assert.True(dataStore.BuildingDeactivated);
    }

    [Fact]
    public async Task ExecuteAsync_BuildingDoesNotExist_ThrowNotFound()
    {
        var dataStore = new FakeBuildingDataStore(null);
        var useCase = new DeactivateBuildingUseCase(dataStore);
        await Assert.ThrowsAsync<BuildingNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}