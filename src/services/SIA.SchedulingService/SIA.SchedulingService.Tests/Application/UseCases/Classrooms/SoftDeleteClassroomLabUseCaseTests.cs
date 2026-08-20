using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.Classrooms;

public sealed class SoftDeleteClassroomLabUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldSoftDeleteClassroomLab()
    {
        var existingLab = new ClassroomLab(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "LAB-01", "Lab", 30, "Desc");
        var dataStore = new FakeClassroomLabDataStore(existingLab);
        var useCase = new SoftDeleteClassroomLabUseCase(dataStore);

        var response = await useCase.ExecuteAsync(existingLab.TenantId, existingLab.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(response.Status);
        Assert.True(dataStore.ClassroomLabDeleted);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLabDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomLabDataStore(null);
        var useCase = new SoftDeleteClassroomLabUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomLabNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}