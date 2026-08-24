using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

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
            TenantId = tenantId,
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Laboratorio Redes",
            Capacity = 30,
            Description = "Desc"
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

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
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateClassroomLabCodeException()
    {
        var dataStore = new FakeClassroomLabDataStore { CodeExistsResult = true };
        var useCase = new CreateClassroomLabUseCase(dataStore);

        var request = new CreateClassroomLabRequest
        {
            TenantId = Guid.NewGuid(),
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Lab",
            Capacity = 30
        };

        await Assert.ThrowsAsync<DuplicateClassroomLabCodeException>(() =>
            useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedClassroomLab);
        Assert.Null(dataStore.AddedEvent);
    }
}