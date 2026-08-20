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
        var existingSchedule = new ClassSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "MARTES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        existingSchedule.SoftDelete();

        var dataStore = new FakeClassScheduleDataStore(existingSchedule);
        var useCase = new RestoreClassScheduleUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.Status);
        Assert.True(dataStore.ScheduleRestored);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassScheduleDataStore(null);
        var useCase = new RestoreClassScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<ClassScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}