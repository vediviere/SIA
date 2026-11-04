using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.TeacherCompetencies;

public sealed class ActivateTeacherCompetencyUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingCompetency_ShouldActivate()
    {
        var competency = new TeacherCompetency(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Avanzado", "Validado");
        competency.Deactivate();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeTeacherCompetencyDataStore { TeacherCompetencyById = competency };
        var useCase = new ActivateTeacherCompetencyUseCase(dataStore);

        await useCase.ExecuteAsync(competency.TenantId, competency.TeacherCompetencyId, correlationId, CancellationToken.None);

        Assert.True(competency.Status);
        Assert.NotNull(dataStore.ActivatedTeacherCompetency);
        Assert.Equal(competency.TeacherCompetencyId, dataStore.ActivatedTeacherCompetency.TeacherCompetencyId);
        Assert.NotNull(dataStore.ActivatedEvent);
        Assert.Equal(competency.TeacherCompetencyId, dataStore.ActivatedEvent.TeacherCompetencyId);
        Assert.Equal(competency.TenantId, dataStore.ActivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.ActivatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeTeacherCompetencyDataStore { TeacherCompetencyById = null };
        var useCase = new ActivateTeacherCompetencyUseCase(dataStore);

        await Assert.ThrowsAsync<TeacherCompetencyNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.ActivatedTeacherCompetency);
        Assert.Null(dataStore.ActivatedEvent);
    }
}