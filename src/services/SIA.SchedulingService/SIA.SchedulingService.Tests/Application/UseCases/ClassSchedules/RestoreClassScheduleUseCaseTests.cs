using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassSchedules;

public sealed class RestoreClassScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldRestoreClassSchedule()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingSchedule = new ClassSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "MARTES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        existingSchedule.SoftDelete();

        var dataStore = new FakeClassScheduleDataStore(existingSchedule);
        var useCase = new RestoreClassScheduleUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, existingSchedule.Id, correlationId, CancellationToken.None);

        Assert.NotNull(dataStore.RestoredSchedule);
        Assert.True(dataStore.RestoredSchedule.Status);
        Assert.NotNull(dataStore.RestoredEvent);
        Assert.Equal(existingSchedule.Id, dataStore.RestoredEvent.ClassScheduleId);
        Assert.Equal(correlationId, dataStore.RestoredEvent.CorrelationId);
        Assert.Equal(1, dataStore.RestoredEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassScheduleDataStore(null);
        var useCase = new RestoreClassScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<ClassScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.RestoredSchedule);
        Assert.Null(dataStore.RestoredEvent);
    }
}