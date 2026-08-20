using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class DeactivateTeachingSupportHoursUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidSupportHours_DeactivateAndPublishEvent()
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
        Assert.True(dataStore.SupportHoursDeactivated);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotExist_ThrowNotFoundException()
    {
        var dataStore = new FakeTeachingSupportHoursDataStore(null);
        var useCase = new DeactivateTeachingSupportHoursUseCase(dataStore);

        await Assert.ThrowsAsync<TeachingSupportHoursNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}