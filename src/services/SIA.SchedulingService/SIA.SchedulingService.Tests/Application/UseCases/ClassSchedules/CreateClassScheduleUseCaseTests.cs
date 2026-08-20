using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Contracts.Requests.ClassSchedule;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassSchedules;

public sealed class CreateClassScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateClassSchedule()
    {
        var dataStore = new FakeClassScheduleDataStore();
        var useCase = new CreateClassScheduleUseCase(dataStore);

        var request = new CreateClassScheduleRequest
        {
            TenantId = Guid.NewGuid(),
            OfferingId = Guid.NewGuid(),
            ClassroomLabId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            Day = "MARTES",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        };

        var response = await useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("MARTES", response.Day);
        Assert.True(dataStore.ScheduleAdded);
    }
}