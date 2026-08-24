using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Contracts.Requests.Professors;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Professors;

public sealed class UpdateTeacherUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateTeacher()
    {
        var teacher = new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil viejo", "Tipo viejo", 20);
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeTeacherDataStore { TeacherById = teacher };
        var useCase = new UpdateTeacherUseCase(dataStore);

        var response = await useCase.ExecuteAsync(
            teacher.TenantId,
            teacher.Id,
            new UpdateTeacherRequest
            {
                ProfessionalProfile = "Nuevo perfil",
                ContractType = "Tiempo completo",
                ContractHours = 40
            },
            correlationId,
            CancellationToken.None);

        Assert.Equal("Nuevo perfil", response.ProfessionalProfile);
        Assert.Equal(40, response.ContractHours);
        Assert.NotNull(dataStore.UpdatedTeacher);
        Assert.Equal("Nuevo perfil", dataStore.UpdatedTeacher.ProfessionalProfile);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(teacher.Id, dataStore.UpdatedEvent.ProfessorId);
        Assert.Equal(teacher.TenantId, dataStore.UpdatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTeacherNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeTeacherDataStore { TeacherById = null };
        var useCase = new UpdateTeacherUseCase(dataStore);

        await Assert.ThrowsAsync<TeacherNotFoundException>(() => useCase.ExecuteAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateTeacherRequest
            {
                ProfessionalProfile = "Perfil",
                ContractType = "Tipo",
                ContractHours = 40
            },
            Guid.NewGuid(),
            CancellationToken.None));

        Assert.Null(dataStore.UpdatedTeacher);
        Assert.Null(dataStore.UpdatedEvent);
    }
}