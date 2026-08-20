using SIA.SchedulingService.Application.UseCases.SupportActivities;
using SIA.SchedulingService.Contracts.Requests.SupportActivity;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SIA.SchedulingService.Tests.Application.UseCases.SupportActivities;

public sealed class CreateSupportActivityUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateSupportActivity()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeSupportActivityDataStore();
        var useCase = new CreateSupportActivityUseCase(dataStore);

        var request = new CreateSupportActivityRequest
        {
            TenantId = tenantId,
            Activity = "Asesoría de BD",
            Observation = "Observación de prueba"
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("Asesoría de BD", response.Activity);
        Assert.Equal("Observación de prueba", response.Observation);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);

        Assert.True(dataStore.ActivityAdded);
        Assert.NotNull(dataStore.AddedActivity);
        Assert.Equal(tenantId, dataStore.AddedActivity.TenantId);

        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(dataStore.AddedActivity.Id, dataStore.AddedEvent.SupportActivityId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidData_ShouldThrowArgumentException()
    {
        var dataStore = new FakeSupportActivityDataStore();
        var useCase = new CreateSupportActivityUseCase(dataStore);

        var request = new CreateSupportActivityRequest
        {
            TenantId = Guid.Empty, 
            Activity = "Asesoría de BD",
            Observation = "Observación de prueba"
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));
    }
}