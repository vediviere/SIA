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
        var correlationId = Guid.NewGuid();
        var existingSchedule = new ClassSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "MARTES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var dataStore = new FakeClassScheduleDataStore(existingSchedule);
        var useCase = new SoftDeleteClassScheduleUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, correlationId, CancellationToken.None);

        Assert.False(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.DeletedSchedule);
        Assert.False(dataStore.DeletedSchedule.Status);
        Assert.NotNull(dataStore.DeletedEvent);
        Assert.Equal(existingSchedule.Id, dataStore.DeletedEvent.ClassScheduleId);
        Assert.Equal(correlationId, dataStore.DeletedEvent.CorrelationId);
        Assert.Equal(1, dataStore.DeletedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassScheduleDataStore(null);
        var useCase = new SoftDeleteClassScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<ClassScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeletedSchedule);
        Assert.Null(dataStore.DeletedEvent);
    }
}