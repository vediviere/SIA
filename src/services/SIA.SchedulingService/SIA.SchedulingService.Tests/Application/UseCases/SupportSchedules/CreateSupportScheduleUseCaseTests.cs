using SIA.SchedulingService.Application.UseCases.SupportSchedules;
using SIA.SchedulingService.Contracts.Requests.SupportSchedules;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportSchedules;

public sealed class CreateSupportScheduleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateSupportSchedule()
    {
        var dataStore = new FakeSupportScheduleDataStore();
        var useCase = new CreateSupportScheduleUseCase(dataStore);

        var request = new CreateSupportScheduleRequest
        {
            TenantId = Guid.NewGuid(),
            SupportHourId = Guid.NewGuid(),
            ClassroomLabId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            Day = "LUNES",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        };

        var response = await useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("LUNES", response.Day);
        Assert.True(dataStore.ScheduleAdded);
    }
}