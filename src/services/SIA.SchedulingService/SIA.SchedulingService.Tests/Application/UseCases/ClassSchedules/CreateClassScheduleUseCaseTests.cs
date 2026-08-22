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
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeClassScheduleDataStore();
        var useCase = new CreateClassScheduleUseCase(dataStore);

        var request = new CreateClassScheduleRequest
        {
            TenantId = tenantId,
            OfferingId = Guid.NewGuid(),
            ClassroomLabId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            Day = "MARTES",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedSchedule);
        Assert.Equal("MARTES", dataStore.AddedSchedule.Day);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(response.Id, dataStore.AddedEvent.ClassScheduleId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }
}