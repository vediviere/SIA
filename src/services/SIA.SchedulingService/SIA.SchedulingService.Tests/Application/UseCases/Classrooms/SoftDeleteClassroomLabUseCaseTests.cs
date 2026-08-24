using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.Classrooms;

public sealed class SoftDeleteClassroomLabUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldSoftDeleteClassroomLab()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingLab = new ClassroomLab(tenantId, Guid.NewGuid(), Guid.NewGuid(), "LAB-01", "Lab", 30, "Desc");
        var dataStore = new FakeClassroomLabDataStore(existingLab);
        var useCase = new SoftDeleteClassroomLabUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingLab.Id, correlationId, CancellationToken.None);

        Assert.False(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.DeletedClassroomLab);
        Assert.False(dataStore.DeletedClassroomLab.Status);
        Assert.NotNull(dataStore.DeletedEvent);
        Assert.Equal(existingLab.Id, dataStore.DeletedEvent.ClassroomLabId);
        Assert.Equal(correlationId, dataStore.DeletedEvent.CorrelationId);
        Assert.Equal(1, dataStore.DeletedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLabDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomLabDataStore(null);
        var useCase = new SoftDeleteClassroomLabUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomLabNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeletedClassroomLab);
        Assert.Null(dataStore.DeletedEvent);
    }
}