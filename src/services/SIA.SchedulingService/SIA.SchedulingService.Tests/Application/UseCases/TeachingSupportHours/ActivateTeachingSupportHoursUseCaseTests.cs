using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class ActivateTeachingSupportHoursUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidSupportHours_ActivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var supportHourId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var tsh = new TeachingSupportHour(tenantId, Guid.NewGuid(), Guid.NewGuid(), 5);
        tsh.Deactivate();

        var dataStore = new FakeTeachingSupportHoursDataStore(tsh);
        var useCase = new ActivateTeachingSupportHoursUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, supportHourId, correlationId, CancellationToken.None);

        Assert.True(tsh.Status);
        Assert.NotNull(tsh.UpdatedAtUtc);
        Assert.True(dataStore.SupportHoursActivated);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotExist_ThrowNotFoundException()
    {
        var dataStore = new FakeTeachingSupportHoursDataStore(null);
        var useCase = new ActivateTeachingSupportHoursUseCase(dataStore);

        await Assert.ThrowsAsync<TeachingSupportHoursNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}