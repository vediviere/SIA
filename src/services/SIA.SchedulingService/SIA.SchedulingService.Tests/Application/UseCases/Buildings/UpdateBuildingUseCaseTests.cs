using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Buildings;

public sealed class UpdateBuildingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidData_UpdateBuilding()
    {
        var tenantId = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var existingBuilding = new Building(tenantId, "A1", "Edificio A", "EDIFICIO A-ISIC");

        var dataStore = new FakeBuildingDataStore(existingBuilding);
        var useCase = new UpdateBuildingUseCase(dataStore);

        var request = new UpdateBuildingRequest
        {
            Code = "  a2  ",
            Name = "Edificio A ISIC-IFOR",
            Description = "EDIFICIO A-ISIC-IFOR"
        };
        var responseBuilding = await useCase.ExecuteAsync(tenantId, buildingId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, responseBuilding.TenantId);
        Assert.Equal("A2", responseBuilding.Code);
        Assert.Equal("Edificio A ISIC-IFOR", responseBuilding.Name);
        Assert.Equal("EDIFICIO A-ISIC-IFOR", responseBuilding.Description);
        Assert.NotNull(responseBuilding.UpdatedAtUtc);
        Assert.Equal(correlationId, responseBuilding.CorrelationId);
        Assert.True(dataStore.BuildingUpdated);
    }

    [Fact]
    public async Task ExecuteAsync_BuildingDoesNotExist_ThrowNotFound()
    {
        var dataStore = new FakeBuildingDataStore(null);
        var useCase = new UpdateBuildingUseCase(dataStore);

        var request = new UpdateBuildingRequest
        {
            Code = "A1",
            Name = "Edificio A",
            Description = "EDIFICIO A-ISIC"
        };
        await Assert.ThrowsAsync<BuildingNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_NewCodeAlreadyExists_ThrowConflict()
    {
        var tenantId = Guid.NewGuid();
        var existingBuilding = new Building(tenantId, "A1", "Edificio A", "Descripción");

        var dataStore = new FakeBuildingDataStore(existingBuilding)
        {
            CodeExistsResult = true
        };
        var useCase = new UpdateBuildingUseCase(dataStore);

        var request = new UpdateBuildingRequest
        {
            Code = "A2",
            Name = "Edificio A",
            Description = "Descripción"
        };
        await Assert.ThrowsAsync<DuplicateBuildingCodeException>(() => useCase.ExecuteAsync(tenantId, Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_NullDescription_UpdateBuildingWithEmptyDescription()
    {
        var tenantId = Guid.NewGuid();
        var existingBuilding = new Building(tenantId, "A1", "Edificio A", "Descripción vieja");

        var dataStore = new FakeBuildingDataStore(existingBuilding);
        var useCase = new UpdateBuildingUseCase(dataStore);

        var request = new UpdateBuildingRequest
        {
            Code = "A1",
            Name = "Edificio A",
            Description = null
        };
        var responseBuilding = await useCase.ExecuteAsync(tenantId, Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None);
        Assert.Equal(string.Empty, responseBuilding.Description);
    }
}