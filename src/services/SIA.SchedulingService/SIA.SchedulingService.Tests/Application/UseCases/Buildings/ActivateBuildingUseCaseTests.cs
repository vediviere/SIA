using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Buildings;

public sealed class ActivateBuildingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidBuilding_ShouldActivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var building = new Building(tenantId, "A1", "Edificio A", "EDIFICIO A-ISIC");
        building.Deactivate();

        var dataStore = new FakeBuildingDataStore(building);
        var useCase = new ActivateBuildingUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, buildingId, correlationId, CancellationToken.None);

        Assert.True(building.Status);
        Assert.NotNull(building.UpdatedAtUtc);

        Assert.NotNull(dataStore.AddedActivatedEvent);
        Assert.Equal(correlationId, dataStore.AddedActivatedEvent.CorrelationId);
        Assert.True(dataStore.AddedActivatedEvent.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBuildingDoesNotExist_ShouldThrowBuildingNotFoundException()
    {
        var dataStore = new FakeBuildingDataStore(null);
        var useCase = new ActivateBuildingUseCase(dataStore);
        await Assert.ThrowsAsync<BuildingNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
        Assert.Null(dataStore.AddedActivatedEvent);
    }
}