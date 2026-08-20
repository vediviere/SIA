using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Buildings;

public sealed class ActivateBuildingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidBuilding_ActivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var building = new Building(tenantId, "A1", "Edificio A", "Descripción");
        building.Deactivate();

        var dataStore = new FakeBuildingDataStore(building);
        var useCase = new ActivateBuildingUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, buildingId, correlationId, CancellationToken.None);

        Assert.True(building.Status);
        Assert.NotNull(building.UpdatedAtUtc);
        Assert.True(dataStore.BuildingActivated);
    }

    [Fact]
    public async Task ExecuteAsync_BuildingDoesNotExist_ThrowNotFound()
    {
        var dataStore = new FakeBuildingDataStore(null);
        var useCase = new ActivateBuildingUseCase(dataStore);
        await Assert.ThrowsAsync<BuildingNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}