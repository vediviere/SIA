using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class UpdateTeachingSupportHoursUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateTeachingSupportHours()
    {
        var tenantId = Guid.NewGuid();
        var supportHourId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var existingTSH = new TeachingSupportHour(tenantId, Guid.NewGuid(), Guid.NewGuid(), 5);

        var dataStore = new FakeTeachingSupportHoursDataStore(existingTSH);
        var useCase = new UpdateTeachingSupportHoursUseCase(dataStore);

        var request = new UpdateTeachingSupportHoursRequest
        {
            Hours = 10
        };

        var response = await useCase.ExecuteAsync(tenantId, supportHourId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(10, response.Hours);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);

        Assert.NotNull(dataStore.UpdatedTeachingSupportHours);
        Assert.Equal(10, dataStore.UpdatedTeachingSupportHours.Hours);

        Assert.NotNull(dataStore.AddedUpdatedEvent);
        Assert.Equal(correlationId, dataStore.AddedUpdatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDoesNotExist_ShouldThrowTeachingSupportHoursNotFoundException()
    {
        var dataStore = new FakeTeachingSupportHoursDataStore(null);
        var useCase = new UpdateTeachingSupportHoursUseCase(dataStore);
        var request = new UpdateTeachingSupportHoursRequest { Hours = 10 };

        await Assert.ThrowsAsync<TeachingSupportHoursNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedTeachingSupportHours);
        Assert.Null(dataStore.AddedUpdatedEvent);
    }
}