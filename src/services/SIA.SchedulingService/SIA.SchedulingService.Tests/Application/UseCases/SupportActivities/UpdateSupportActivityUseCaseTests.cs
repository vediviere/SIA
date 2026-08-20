using SIA.SchedulingService.Application.UseCases.SupportActivities;
using SIA.SchedulingService.Contracts.Requests.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportActivities;

public sealed class UpdateSupportActivityUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateActivity()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingActivity = new SupportActivity(tenantId, "Actividad Vieja", "Observación Vieja");
        var dataStore = new FakeSupportActivityDataStore(existingActivity);
        var useCase = new UpdateSupportActivityUseCase(dataStore);

        var request = new UpdateSupportActivityRequest
        {
            Activity = "Actividad Nueva",
            Observation = "Observación Nueva"
        };

        var response = await useCase.ExecuteAsync(
            existingActivity.Id,
            tenantId,
            request,
            correlationId,
            CancellationToken.None);

        Assert.Equal("Actividad Nueva", response.Activity);
        Assert.Equal("Observación Nueva", response.Observation);

        Assert.True(dataStore.ActivityUpdated);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(existingActivity.Id, dataStore.UpdatedEvent.SupportActivityId);
    }
}