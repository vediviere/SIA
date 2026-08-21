using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class DeactivateAcademicOfferingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidOffering_ShouldDeactivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var offeringId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var offering = new AcademicOffering(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OFERTADA");

        var dataStore = new FakeAcademicOfferingDataStore(offering);
        var useCase = new DeactivateAcademicOfferingUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, offeringId, correlationId, CancellationToken.None);

        Assert.False(offering.Status);
        Assert.NotNull(offering.UpdatedAtUtc);

        Assert.NotNull(dataStore.AddedDeactivatedEvent);
        Assert.Equal(correlationId, dataStore.AddedDeactivatedEvent.CorrelationId);
        Assert.False(dataStore.AddedDeactivatedEvent.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOfferingDoesNotExist_ShouldThrowAcademicOfferingNotFoundException()
    {
        var dataStore = new FakeAcademicOfferingDataStore(null);
        var useCase = new DeactivateAcademicOfferingUseCase(dataStore);
        await Assert.ThrowsAsync<AcademicOfferingNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
        Assert.Null(dataStore.AddedDeactivatedEvent);
    }
}