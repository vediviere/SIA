using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class DeactivateTeachingSupportHoursUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSupportHours_ShouldDeactivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var supportHourId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var tsh = new TeachingSupportHour(tenantId, Guid.NewGuid(), Guid.NewGuid(), 5);

        var dataStore = new FakeTeachingSupportHoursDataStore(tsh);
        var useCase = new DeactivateTeachingSupportHoursUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, supportHourId, correlationId, CancellationToken.None);

        Assert.False(tsh.Status);
        Assert.NotNull(tsh.UpdatedAtUtc);

        Assert.NotNull(dataStore.AddedDeactivatedEvent);
        Assert.Equal(correlationId, dataStore.AddedDeactivatedEvent.CorrelationId);
        Assert.False(dataStore.AddedDeactivatedEvent.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDoesNotExist_ShouldThrowTeachingSupportHoursNotFoundException()
    {
        var dataStore = new FakeTeachingSupportHoursDataStore(null);
        var useCase = new DeactivateTeachingSupportHoursUseCase(dataStore);
        await Assert.ThrowsAsync<TeachingSupportHoursNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
        Assert.Null(dataStore.AddedDeactivatedEvent);
    }
}