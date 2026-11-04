using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.TeacherCompetencies;

public sealed class DeactivateTeacherCompetencyUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingCompetency_ShouldDeactivate()
    {
        var competency = new TeacherCompetency(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Avanzado", "Validado");
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeTeacherCompetencyDataStore { TeacherCompetencyById = competency };
        var useCase = new DeactivateTeacherCompetencyUseCase(dataStore);

        await useCase.ExecuteAsync(competency.TenantId, competency.TeacherCompetencyId, correlationId, CancellationToken.None);

        Assert.False(competency.Status);
        Assert.NotNull(dataStore.DeactivatedTeacherCompetency);
        Assert.Equal(competency.TeacherCompetencyId, dataStore.DeactivatedTeacherCompetency.TeacherCompetencyId);
        Assert.NotNull(dataStore.DeactivatedEvent);
        Assert.Equal(competency.TeacherCompetencyId, dataStore.DeactivatedEvent.TeacherCompetencyId);
        Assert.Equal(competency.TenantId, dataStore.DeactivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.DeactivatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeTeacherCompetencyDataStore { TeacherCompetencyById = null };
        var useCase = new DeactivateTeacherCompetencyUseCase(dataStore);

        await Assert.ThrowsAsync<TeacherCompetencyNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeactivatedTeacherCompetency);
        Assert.Null(dataStore.DeactivatedEvent);
    }
}