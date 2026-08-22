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
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeSupportScheduleDataStore();
        var useCase = new CreateSupportScheduleUseCase(dataStore);

        var request = new CreateSupportScheduleRequest
        {
            TenantId = tenantId,
            SupportHourId = Guid.NewGuid(),
            ClassroomLabId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            Day = "LUNES",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedSchedule);
        Assert.Equal("LUNES", dataStore.AddedSchedule.Day);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(response.Id, dataStore.AddedEvent.SupportScheduleId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }
}