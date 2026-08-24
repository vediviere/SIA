using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.Classrooms;

public sealed class RestoreClassroomLabUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldRestoreClassroomLab()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingLab = new ClassroomLab(tenantId, Guid.NewGuid(), Guid.NewGuid(), "LAB-01", "Lab", 30, "Desc");
        existingLab.SoftDelete();

        var dataStore = new FakeClassroomLabDataStore(existingLab);
        var useCase = new RestoreClassroomLabUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingLab.Id, correlationId, CancellationToken.None);

        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.RestoredClassroomLab);
        Assert.True(dataStore.RestoredClassroomLab.Status);
        Assert.NotNull(dataStore.RestoredEvent);
        Assert.Equal(existingLab.Id, dataStore.RestoredEvent.ClassroomLabId);
        Assert.Equal(correlationId, dataStore.RestoredEvent.CorrelationId);
        Assert.Equal(1, dataStore.RestoredEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLabDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomLabDataStore(null);
        var useCase = new RestoreClassroomLabUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomLabNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.RestoredClassroomLab);
        Assert.Null(dataStore.RestoredEvent);
    }
}