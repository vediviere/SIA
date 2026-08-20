using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Contracts.Requests.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassSchedules;

public sealed class UpdateClassScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateClassSchedule()
    {
        var tenantId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(2);
        var existingSchedule = new ClassSchedule(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "MARTES", startTime, endTime);

        var dataStore = new FakeClassScheduleDataStore(existingSchedule);
        var useCase = new UpdateClassScheduleUseCase(dataStore);

        var request = new UpdateClassScheduleRequest
        {
            Day = "MIERCOLES",
            StartTime = startTime.AddDays(1),
            EndTime = endTime.AddDays(1)
        };

        var response = await useCase.ExecuteAsync(tenantId, existingSchedule.Id, request, Guid.NewGuid(), CancellationToken.None);

        Assert.Equal("MIERCOLES", response.Day);
        Assert.True(dataStore.ScheduleUpdated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScheduleDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassScheduleDataStore(null);
        var useCase = new UpdateClassScheduleUseCase(dataStore);

        var request = new UpdateClassScheduleRequest { Day = "MIERCOLES", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };

        await Assert.ThrowsAsync<ClassScheduleNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));
    }
}