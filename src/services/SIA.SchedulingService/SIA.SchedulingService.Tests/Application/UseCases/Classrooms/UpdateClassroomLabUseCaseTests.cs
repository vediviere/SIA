using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.Classrooms;

public sealed class UpdateClassroomLabUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateClassroomLab()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingLab = new ClassroomLab(tenantId, Guid.NewGuid(), Guid.NewGuid(), "LAB-01", "Lab", 30, "Desc");
        var dataStore = new FakeClassroomLabDataStore(existingLab);
        var useCase = new UpdateClassroomLabUseCase(dataStore);

        var request = new UpdateClassroomLabRequest
        {
            Code = "LAB-02",
            Name = "Lab Actualizado",
            Capacity = 40,
            Description = "Desc Actualizada"
        };

        var response = await useCase.ExecuteAsync(tenantId, existingLab.Id, request, correlationId, CancellationToken.None);

        Assert.Equal("LAB-02", response.Code);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.UpdatedClassroomLab);
        Assert.Equal("LAB-02", dataStore.UpdatedClassroomLab.Code);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(existingLab.Id, dataStore.UpdatedEvent.ClassroomLabId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
        Assert.Equal(1, dataStore.UpdatedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLabDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomLabDataStore(null);
        var useCase = new UpdateClassroomLabUseCase(dataStore);
        var request = new UpdateClassroomLabRequest { Code = "LAB-02", Name = "Lab", Capacity = 30 };

        await Assert.ThrowsAsync<ClassroomLabNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedClassroomLab);
        Assert.Null(dataStore.UpdatedEvent);
    }
}