using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;
using SIA.SchedulingService.Application.UseCases.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportSchedules;

public sealed class SoftDeleteSupportScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldSoftDeleteSupportSchedule()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingSchedule = new SupportSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "LUNES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var dataStore = new FakeSupportScheduleDataStore(existingSchedule);
        var useCase = new SoftDeleteSupportScheduleUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, existingSchedule.Id, correlationId, CancellationToken.None);

        Assert.NotNull(dataStore.DeletedSchedule);
        Assert.False(dataStore.DeletedSchedule.Status);
        Assert.NotNull(dataStore.DeletedEvent);
        Assert.Equal(existingSchedule.Id, dataStore.DeletedEvent.SupportScheduleId);
        Assert.Equal(correlationId, dataStore.DeletedEvent.CorrelationId);
        Assert.Equal(1, dataStore.DeletedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportScheduleDataStore(null);
        var useCase = new SoftDeleteSupportScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<SupportScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeletedSchedule);
        Assert.Null(dataStore.DeletedEvent);
    }
}