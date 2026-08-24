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
        var correlationId = Guid.NewGuid();
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

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, request, correlationId, CancellationToken.None);

        Assert.Equal("MARTES", response.Day);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.UpdatedSchedule);
        Assert.Equal("MARTES", dataStore.UpdatedSchedule.Day);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(existingSchedule.Id, dataStore.UpdatedEvent.SupportScheduleId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
        Assert.Equal(1, dataStore.UpdatedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportScheduleDataStore(null);
        var useCase = new UpdateSupportScheduleUseCase(dataStore);

        var request = new UpdateSupportScheduleRequest { Day = "MARTES", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };

        await Assert.ThrowsAsync<SupportScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedSchedule);
        Assert.Null(dataStore.UpdatedEvent);
    }
}