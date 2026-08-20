using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassroomTypes;

public sealed class RestoreClassroomTypeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldRestoreClassroomType()
    {
        var tenantId = Guid.NewGuid();
        var existingType = new ClassroomType(tenantId, "LAB", "Lab", "Desc");
        existingType.SoftDelete();

        var dataStore = new FakeClassroomTypeDataStore(existingType);
        var useCase = new RestoreClassroomTypeUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingType.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.Status);
        Assert.True(dataStore.ClassroomTypeRestored);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTypeDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomTypeDataStore(null);
        var useCase = new RestoreClassroomTypeUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomTypeNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}