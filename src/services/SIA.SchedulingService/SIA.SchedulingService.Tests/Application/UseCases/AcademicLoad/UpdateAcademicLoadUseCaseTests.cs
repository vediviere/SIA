using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoads;

public sealed class UpdateAcademicLoadUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidData_UpdateAcademicLoad()
    {
        var tenantId = Guid.NewGuid();
        var academicLoadId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var existingLoad = new AcademicLoad(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-OLD-100", DateTime.UtcNow, 15, 5, DateTime.UtcNow);

        var dataStore = new FakeAcademicLoadDataStore(existingLoad);
        var useCase = new UpdateAcademicLoadUseCase(dataStore);

        var request = new UpdateAcademicLoadRequest
        {
            OfficialLetterNumber = "  OF-NEW-200  ",
            ProposedDate = DateTime.UtcNow,
            ClassHours = 25,
            SupportHours = 8,
            AssignmentDate = DateTime.UtcNow
        };
        var response = await useCase.ExecuteAsync(tenantId, academicLoadId, request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("OF-NEW-200", response.OfficialLetterNumber);
        Assert.Equal(25, response.ClassHours);
        Assert.Equal(8, response.SupportHours);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.AcademicLoadUpdated);
    }

    [Fact]
    public async Task ExecuteAsync_AcademicLoadDoesNotExist_ThrowNotFound()
    {
        var dataStore = new FakeAcademicLoadDataStore(null);
        var useCase = new UpdateAcademicLoadUseCase(dataStore);

        var request = new UpdateAcademicLoadRequest
        {
            OfficialLetterNumber = "OF-NEW-200",
            ProposedDate = DateTime.UtcNow,
            ClassHours = 25,
            SupportHours = 8,
            AssignmentDate = DateTime.UtcNow
        };
        await Assert.ThrowsAsync<AcademicLoadNotFoundException>(() =>  useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));
    }
}