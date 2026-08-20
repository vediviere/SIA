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
        var existingActivity = new SupportActivity(tenantId, "Asesoría", "Observación");
        var dataStore = new FakeSupportActivityDataStore(existingActivity);
        var useCase = new SoftDeleteSupportActivityUseCase(dataStore);

        await useCase.ExecuteAsync(existingActivity.Id, tenantId, Guid.NewGuid(), CancellationToken.None);

        Assert.False(existingActivity.Status); 
        Assert.True(dataStore.ActivityDeleted); 
        Assert.NotNull(dataStore.DeletedEvent);
    }
}