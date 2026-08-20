using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoads;

public sealed class DeactivateAcademicLoadUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidAcademicLoad_DeactivateAndPublishEvent()
    {
        var tenantId = Guid.NewGuid();
        var academicLoadId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicLoad = new AcademicLoad(tenantId,Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),"OF-2026-001",DateTime.UtcNow,20,10,DateTime.UtcNow);

        var dataStore = new FakeAcademicLoadDataStore(academicLoad);
        var useCase = new DeactivateAcademicLoadUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, academicLoadId, correlationId, CancellationToken.None);

        Assert.False(academicLoad.Status);
        Assert.NotNull(academicLoad.UpdatedAtUtc);
        Assert.True(dataStore.AcademicLoadDeactivated);
    }

    [Fact]
    public async Task ExecuteAsync_AcademicLoadDoesNotExist_ThrowNotFound()
    {
        var dataStore = new FakeAcademicLoadDataStore(null);
        var useCase = new DeactivateAcademicLoadUseCase(dataStore);
        await Assert.ThrowsAsync<AcademicLoadNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}