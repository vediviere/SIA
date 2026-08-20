using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassSchedules;

public sealed class SoftDeleteClassScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldSoftDeleteClassSchedule()
    {
        var tenantId = Guid.NewGuid();
        var existingSchedule = new ClassSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "MARTES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var dataStore = new FakeClassScheduleDataStore(existingSchedule);
        var useCase = new SoftDeleteClassScheduleUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(response.Status);
        Assert.True(dataStore.ScheduleDeleted);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassScheduleDataStore(null);
        var useCase = new SoftDeleteClassScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<ClassScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}