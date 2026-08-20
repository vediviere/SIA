using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;
using SIA.SchedulingService.Application.UseCases.SupportSchedules;
using SIA.SchedulingService.Contracts.Requests.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportSchedules;

public sealed class UpdateSupportScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateSupportSchedule()
    {
        var tenantId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(2);
        var existingSchedule = new SupportSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "LUNES", startTime, endTime);

        var dataStore = new FakeSupportScheduleDataStore(existingSchedule);
        var useCase = new UpdateSupportScheduleUseCase(dataStore);

        var request = new UpdateSupportScheduleRequest
        {
            Day = "MARTES",
            StartTime = startTime.AddDays(1),
            EndTime = endTime.AddDays(1)
        };

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, request, Guid.NewGuid(), CancellationToken.None);

        Assert.Equal("MARTES", response.Day);
        Assert.True(dataStore.ScheduleUpdated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportScheduleDataStore(null);
        var useCase = new UpdateSupportScheduleUseCase(dataStore);

        var request = new UpdateSupportScheduleRequest { Day = "MARTES", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };

        await Assert.ThrowsAsync<SupportScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));
    }
}