using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Professors;

public sealed class ActivateTeacherUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingTeacher_ShouldActivate()
    {
        var teacher = new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil", "Tipo", 40);
        teacher.Deactivate();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeTeacherDataStore { TeacherById = teacher };
        var useCase = new ActivateTeacherUseCase(dataStore);

        await useCase.ExecuteAsync(teacher.TenantId, teacher.Id, correlationId, CancellationToken.None);

        Assert.True(teacher.Status);
        Assert.NotNull(dataStore.ActivatedTeacher);
        Assert.Equal(teacher.Id, dataStore.ActivatedTeacher.Id);
        Assert.NotNull(dataStore.ActivatedEvent);
        Assert.Equal(teacher.Id, dataStore.ActivatedEvent.ProfessorId);
        Assert.Equal(teacher.TenantId, dataStore.ActivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.ActivatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTeacherNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeTeacherDataStore { TeacherById = null };
        var useCase = new ActivateTeacherUseCase(dataStore);

        await Assert.ThrowsAsync<TeacherNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.ActivatedTeacher);
        Assert.Null(dataStore.ActivatedEvent);
    }
}