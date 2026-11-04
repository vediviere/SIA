using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies;
using SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.TeacherCompetencies;

public sealed class CreateTeacherCompetencyUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreate()
    {
        var tenantId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var competencyId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeTeacherCompetencyDataStore();
        var useCase = new CreateTeacherCompetencyUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, teacherId, new CreateTeacherCompetencyRequest
        {
            CompetencyId = competencyId,
            Level = "Avanzado",
            Stage = "Validado"
        }, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(teacherId, response.TeacherId);
        Assert.Equal(competencyId, response.CompetencyId);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedTeacherCompetency);
        Assert.Equal(competencyId, dataStore.AddedTeacherCompetency.CompetencyId);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(dataStore.AddedTeacherCompetency.TeacherCompetencyId, dataStore.AddedEvent.TeacherCompetencyId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAlreadyExists_ShouldThrowConflict()
    {
        var tenantId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var dataStore = new FakeTeacherCompetencyDataStore { CompetencyAlreadyExistsResult = true };
        var useCase = new CreateTeacherCompetencyUseCase(dataStore);

        await Assert.ThrowsAsync<DuplicateTeacherCompetencyException>(() => useCase.ExecuteAsync(tenantId, teacherId, new CreateTeacherCompetencyRequest
        {
            CompetencyId = Guid.NewGuid(),
            Level = "Avanzado",
            Stage = "Validado"
        }, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedTeacherCompetency);
        Assert.Null(dataStore.AddedEvent);
    }
}