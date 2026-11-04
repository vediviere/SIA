using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies;
using SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.TeacherCompetencies;

public sealed class UpdateTeacherCompetencyUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdate()
    {
        var competency = new TeacherCompetency(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Básico", "Pendiente");
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeTeacherCompetencyDataStore { TeacherCompetencyById = competency };
        var useCase = new UpdateTeacherCompetencyUseCase(dataStore);

        var response = await useCase.ExecuteAsync(
            competency.TenantId,
            competency.TeacherCompetencyId,
            new UpdateTeacherCompetencyRequest
            {
                Level = "Avanzado",
                Stage = "Validado"
            },
            correlationId,
            CancellationToken.None);

        Assert.Equal("Avanzado", response.Level);
        Assert.Equal("Validado", response.Stage);
        Assert.NotNull(dataStore.UpdatedTeacherCompetency);
        Assert.Equal("Avanzado", dataStore.UpdatedTeacherCompetency.Level);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(competency.TeacherCompetencyId, dataStore.UpdatedEvent.TeacherCompetencyId);
        Assert.Equal(competency.TenantId, dataStore.UpdatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeTeacherCompetencyDataStore { TeacherCompetencyById = null };
        var useCase = new UpdateTeacherCompetencyUseCase(dataStore);

        await Assert.ThrowsAsync<TeacherCompetencyNotFoundException>(() => useCase.ExecuteAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateTeacherCompetencyRequest
            {
                Level = "Avanzado",
                Stage = "Validado"
            },
            Guid.NewGuid(),
            CancellationToken.None));

        Assert.Null(dataStore.UpdatedTeacherCompetency);
        Assert.Null(dataStore.UpdatedEvent);
    }
}