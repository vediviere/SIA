using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;
using SIA.SchedulingService.Application.UseCases.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportSchedules;

public sealed class RestoreSupportScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldRestoreSupportSchedule()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingSchedule = new SupportSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "LUNES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        existingSchedule.SoftDelete();

        var dataStore = new FakeSupportScheduleDataStore(existingSchedule);
        var useCase = new RestoreSupportScheduleUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, correlationId, CancellationToken.None);

        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.RestoredSchedule);
        Assert.True(dataStore.RestoredSchedule.Status);
        Assert.NotNull(dataStore.RestoredEvent);
        Assert.Equal(existingSchedule.Id, dataStore.RestoredEvent.SupportScheduleId);
        Assert.Equal(correlationId, dataStore.RestoredEvent.CorrelationId);
        Assert.Equal(1, dataStore.RestoredEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportScheduleDataStore(null);
        var useCase = new RestoreSupportScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<SupportScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.RestoredSchedule);
        Assert.Null(dataStore.RestoredEvent);
    }
}