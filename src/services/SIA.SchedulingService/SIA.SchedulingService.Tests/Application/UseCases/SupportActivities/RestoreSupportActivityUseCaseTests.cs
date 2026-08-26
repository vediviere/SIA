using SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;
using SIA.SchedulingService.Application.UseCases.SupportActivities;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportActivities;

public sealed class RestoreSupportActivityUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldRestoreActivity()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingActivity = new SupportActivity(tenantId, "Tutoría", "Obs");
        existingActivity.SoftDelete();

        var dataStore = new FakeSupportActivityDataStore(existingActivity);
        var useCase = new RestoreSupportActivityUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, existingActivity.Id, correlationId, CancellationToken.None);

        Assert.NotNull(dataStore.RestoredActivity);
        Assert.True(dataStore.RestoredActivity.Status);
        Assert.NotNull(dataStore.RestoredEvent);
        Assert.Equal(existingActivity.Id, dataStore.RestoredEvent.SupportActivityId);
        Assert.Equal(correlationId, dataStore.RestoredEvent.CorrelationId);
        Assert.Equal(1, dataStore.RestoredEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenActivityDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportActivityDataStore(null);
        var useCase = new RestoreSupportActivityUseCase(dataStore);

        await Assert.ThrowsAsync<SupportActivityNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.RestoredActivity);
        Assert.Null(dataStore.RestoredEvent);
    }
}