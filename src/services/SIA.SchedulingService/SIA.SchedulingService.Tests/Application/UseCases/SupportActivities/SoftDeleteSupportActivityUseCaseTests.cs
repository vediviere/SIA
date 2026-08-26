using SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;
using SIA.SchedulingService.Application.UseCases.SupportActivities;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportActivities;

public sealed class SoftDeleteSupportActivityUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldSoftDeleteActivity()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingActivity = new SupportActivity(tenantId, "Tutoría", "Obs");
        var dataStore = new FakeSupportActivityDataStore(existingActivity);
        var useCase = new SoftDeleteSupportActivityUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, existingActivity.Id, correlationId, CancellationToken.None);

        Assert.NotNull(dataStore.DeletedActivity);
        Assert.False(dataStore.DeletedActivity.Status);
        Assert.NotNull(dataStore.DeletedEvent);
        Assert.Equal(existingActivity.Id, dataStore.DeletedEvent.SupportActivityId);
        Assert.Equal(correlationId, dataStore.DeletedEvent.CorrelationId);
        Assert.Equal(1, dataStore.DeletedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenActivityDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportActivityDataStore(null);
        var useCase = new SoftDeleteSupportActivityUseCase(dataStore);

        await Assert.ThrowsAsync<SupportActivityNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeletedActivity);
        Assert.Null(dataStore.DeletedEvent);
    }
}