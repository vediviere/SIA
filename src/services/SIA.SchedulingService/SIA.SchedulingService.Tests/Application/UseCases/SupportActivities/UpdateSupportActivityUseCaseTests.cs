using SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;
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
        var existingActivity = new SupportActivity(tenantId, "Tutoría", "Observación inicial");

        var dataStore = new FakeSupportActivityDataStore(existingActivity);
        var useCase = new UpdateSupportActivityUseCase(dataStore);

        var request = new UpdateSupportActivityRequest
        {
            Activity = "Tutoría Actualizada",
            Observation = "Observación editada"
        };

        var response = await useCase.ExecuteAsync(tenantId, existingActivity.Id, request, correlationId, CancellationToken.None);

        Assert.Equal("Tutoría Actualizada", response.Activity);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.UpdatedActivity);
        Assert.Equal("Tutoría Actualizada", dataStore.UpdatedActivity.Activity);
        Assert.Equal("Observación editada", dataStore.UpdatedActivity.Observation);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(existingActivity.Id, dataStore.UpdatedEvent.SupportActivityId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
        Assert.Equal(1, dataStore.UpdatedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenActivityDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeSupportActivityDataStore(null);
        var useCase = new UpdateSupportActivityUseCase(dataStore);

        var request = new UpdateSupportActivityRequest { Activity = "Act", Observation = "Obs" };

        await Assert.ThrowsAsync<SupportActivityNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedActivity);
        Assert.Null(dataStore.UpdatedEvent);
    }
}