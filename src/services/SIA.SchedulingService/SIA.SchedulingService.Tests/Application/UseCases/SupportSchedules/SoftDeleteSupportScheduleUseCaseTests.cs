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
        var existingSchedule = new SupportSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "LUNES", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var dataStore = new FakeSupportScheduleDataStore(existingSchedule);
        var useCase = new SoftDeleteSupportScheduleUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(response.Status);
        Assert.True(dataStore.ScheduleDeleted);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportScheduleDataStore(null);
        var useCase = new SoftDeleteSupportScheduleUseCase(dataStore);

        await Assert.ThrowsAsync<SupportScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}