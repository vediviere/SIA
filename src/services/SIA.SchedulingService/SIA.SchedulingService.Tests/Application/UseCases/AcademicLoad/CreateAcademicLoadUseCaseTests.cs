using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoads;

public sealed class CreateAcademicLoadUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateAcademicLoad()
    {
        var tenantId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var divisionId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeAcademicLoadDataStore();
        var useCase = new CreateAcademicLoadUseCase(dataStore);

        var request = new CreateAcademicLoadRequest
        {
            TenantId = tenantId,
            TeacherId = teacherId,
            DivisionId = divisionId,
            AcademicPeriodId = academicPeriodId,
            OfficialLetterNumber = "  OF-2026-001  ",
            ProposedDate = DateTime.UtcNow,
            ClassHours = 20,
            SupportHours = 10,
            AssignmentDate = DateTime.UtcNow
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(teacherId, response.TeacherId);
        Assert.Equal(divisionId, response.DivisionId);
        Assert.Equal(academicPeriodId, response.AcademicPeriodId);
        Assert.Equal("OF-2026-001", response.OfficialLetterNumber);
        Assert.Equal(20, response.ClassHours);
        Assert.Equal(10, response.SupportHours);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);

        
        Assert.NotNull(dataStore.AddedAcademicLoad);
        Assert.Equal(tenantId, dataStore.AddedAcademicLoad.TenantId);
        Assert.Equal("OF-2026-001", dataStore.AddedAcademicLoad.OfficialLetterNumber);

        Assert.NotNull(dataStore.AddedCreatedEvent);
        Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
        Assert.Equal(tenantId, dataStore.AddedCreatedEvent.TenantId);
        Assert.Equal(1, dataStore.AddedCreatedEvent.Version);
    }
}