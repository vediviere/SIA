using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Contracts.Requests;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class UpdateAcademicOfferingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidData_UpdateAcademicOffering()
    {
        var tenantId = Guid.NewGuid();
        var offeringId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var existingOffering = new AcademicOffering(tenantId,Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),"NO ACEPTADA");

        var dataStore = new FakeAcademicOfferingDataStore(existingOffering);
        var useCase = new UpdateAcademicOfferingUseCase(dataStore);

        var request = new UpdateAcademicOfferingRequest
        {
            OfferingStatus = "  ACEPTADA  "
        };
        var response = await useCase.ExecuteAsync(tenantId, offeringId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("ACEPTADA", response.OfferingStatus);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.OfferingUpdated);
    }

    [Fact]
    public async Task ExecuteAsync_OfferingDoesNotExist_ThrowNotFound()
    {
        var dataStore = new FakeAcademicOfferingDataStore(null);
        var useCase = new UpdateAcademicOfferingUseCase(dataStore);

        var request = new UpdateAcademicOfferingRequest
        {
            OfferingStatus = "ACEPTADA"
        };
        await Assert.ThrowsAsync<AcademicOfferingNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));
    }
}