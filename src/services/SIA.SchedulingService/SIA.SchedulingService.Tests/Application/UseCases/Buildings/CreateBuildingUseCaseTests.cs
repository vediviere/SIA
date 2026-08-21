using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Buildings;

public sealed class CreateBuildingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateBuilding()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeBuildingDataStore();
        var useCase = new CreateBuildingUseCase(dataStore);

        var request = new CreateBuildingRequest
        {
            TenantId = tenantId,
            Code = "  a1  ",
            Name = "Edificio A",
            Description = "EDIFICIO A-ISIC"
        };
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("A1", response.Code);
        Assert.Equal("Edificio A", response.Name);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);

        Assert.NotNull(dataStore.AddedBuilding);
        Assert.Equal(tenantId, dataStore.AddedBuilding.TenantId);
        Assert.Equal("A1", dataStore.AddedBuilding.Code);

        Assert.NotNull(dataStore.AddedCreatedEvent);
        Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
        Assert.Equal(tenantId, dataStore.AddedCreatedEvent.TenantId);
        Assert.Equal(1, dataStore.AddedCreatedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateBuildingCodeException()
    {
        var dataStore = new FakeBuildingDataStore { CodeExistsResult = true };
        var useCase = new CreateBuildingUseCase(dataStore);

        var request = new CreateBuildingRequest
        {
            TenantId = Guid.NewGuid(),
            Code = "A1",
            Name = "Edificio A",
            Description = "EDIFICIO A-ISIC"
        };
        await Assert.ThrowsAsync<DuplicateBuildingCodeException>(() => useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedBuilding);
        Assert.Null(dataStore.AddedCreatedEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullDescription_ShouldCreateBuildingWithEmptyDescription()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeBuildingDataStore();
        var useCase = new CreateBuildingUseCase(dataStore);

        var request = new CreateBuildingRequest
        {
            TenantId = Guid.NewGuid(),
            Code = "A1",
            Name = "Edificio A",
            Description = null
        };
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);
        Assert.Equal(string.Empty, response.Description);

        Assert.NotNull(dataStore.AddedBuilding);
        Assert.Equal(string.Empty, dataStore.AddedBuilding.Description);
    }
}