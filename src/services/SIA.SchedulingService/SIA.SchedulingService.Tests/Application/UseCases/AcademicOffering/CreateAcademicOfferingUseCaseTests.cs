using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Contracts.Requests.AcademicOffering;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class CreateAcademicOfferingUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidData_CreateAcademicOffering()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var academicLoadId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeAcademicOfferingDataStore();
        var useCase = new CreateAcademicOfferingUseCase(dataStore);

        var request = new CreateAcademicOfferingRequest
        {
            TenantId = tenantId,
            GroupId = groupId,
            SubjectId = subjectId,
            AcademicLoadId = academicLoadId,
            OfferingStatus = "  ACEPTADA  "
        };
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(groupId, response.GroupId);
        Assert.Equal(subjectId, response.SubjectId);
        Assert.Equal(academicLoadId, response.AcademicLoadId);
        Assert.Equal("ACEPTADA", response.OfferingStatus);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.OfferingAdded);
    }

    [Fact]
    public async Task ExecuteAsync_OfferingAlreadyExists_ThrowException()
    {
        var dataStore = new FakeAcademicOfferingDataStore { ExistsResult = true };
        var useCase = new CreateAcademicOfferingUseCase(dataStore);

        var request = new CreateAcademicOfferingRequest
        {
            TenantId = Guid.NewGuid(),
            GroupId = Guid.NewGuid(),
            SubjectId = Guid.NewGuid(),
            AcademicLoadId = Guid.NewGuid(),
            OfferingStatus = "ACEPTADA"
        };
        await Assert.ThrowsAsync<AcademicOfferingAlreadyExistsException>(() => useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));
    }
}