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
        var existingActivity = new SupportActivity(tenantId, "Asesoría", "Observación");
        existingActivity.SoftDelete(); 

        var dataStore = new FakeSupportActivityDataStore(existingActivity);
        var useCase = new RestoreSupportActivityUseCase(dataStore);

        await useCase.ExecuteAsync(existingActivity.Id, tenantId, Guid.NewGuid(), CancellationToken.None);

        Assert.True(existingActivity.Status); 
        Assert.True(dataStore.ActivityRestored); 
        Assert.NotNull(dataStore.RestoredEvent);
    }
}