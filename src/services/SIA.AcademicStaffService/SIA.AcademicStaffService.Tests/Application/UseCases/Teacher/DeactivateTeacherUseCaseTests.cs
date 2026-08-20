using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Professors;

public sealed class DeactivateTeacherUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingTeacher_ShouldDeactivate()
    {
        var teacher = new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil", "Tipo", 40);

        var dataStore = new FakeTeacherDataStore { TeacherById = teacher };
        var useCase = new DeactivateTeacherUseCase(dataStore);

        await useCase.ExecuteAsync(teacher.TenantId, teacher.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(teacher.Status);
        Assert.True(dataStore.TeacherDeactivated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTeacherNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeTeacherDataStore { TeacherById = null };
        var useCase = new DeactivateTeacherUseCase(dataStore);

        await Assert.ThrowsAsync<TeacherNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.TeacherDeactivated);
    }
}