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
            Activity = "Tutoría Académica",
            Observation = "Observación de prueba"
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedActivity);
        Assert.Equal("Tutoría Académica", dataStore.AddedActivity.Activity);
        Assert.Equal("Observación de prueba", dataStore.AddedActivity.Observation);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(response.Id, dataStore.AddedEvent.SupportActivityId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }
}