using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.Classrooms;

public sealed class CreateClassroomLabUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateClassroomLab()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeClassroomLabDataStore();
        var useCase = new CreateClassroomLabUseCase(dataStore);

        var request = new CreateClassroomLabRequest
        {
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Laboratorio Redes",
            Capacity = 30,
            Description = "Desc"
        };

        var response = await useCase.ExecuteAsync(tenantId, request, correlationId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedClassroomLab);
        Assert.Equal("LAB-01", dataStore.AddedClassroomLab.Code);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(response.Id, dataStore.AddedEvent.ClassroomLabId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTenantIdIsDifferent_ShouldHandleContextProperly()
    {
        var tenantId = Guid.NewGuid();
        var differentTenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeClassroomLabDataStore();
        var useCase = new CreateClassroomLabUseCase(dataStore);

        var request = new CreateClassroomLabRequest
        {
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Laboratorio Redes",
            Capacity = 30,
            Description = "Desc"
        };

        var response = await useCase.ExecuteAsync(differentTenantId, request, correlationId, CancellationToken.None);

        Assert.Equal(differentTenantId, response.TenantId);
        Assert.Equal(differentTenantId, dataStore.AddedClassroomLab.TenantId);
        Assert.Equal(differentTenantId, dataStore.AddedEvent.TenantId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateClassroomLabCodeException()
    {
        var dataStore = new FakeClassroomLabDataStore { CodeExistsResult = true };
        var useCase = new CreateClassroomLabUseCase(dataStore);

        var request = new CreateClassroomLabRequest
        {
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Lab",
            Capacity = 30,
            Description = "Desc"
        };

        await Assert.ThrowsAsync<DuplicateClassroomLabCodeException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedClassroomLab);
        Assert.Null(dataStore.AddedEvent);
    }
}