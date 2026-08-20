using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassroomTypes;

public sealed class SoftDeleteClassroomTypeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldSoftDeleteClassroomType()
    {
        var tenantId = Guid.NewGuid();
        var existingType = new ClassroomType(tenantId, "LAB", "Lab", "Desc");
        var dataStore = new FakeClassroomTypeDataStore(existingType);
        var useCase = new SoftDeleteClassroomTypeUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingType.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(response.Status);
        Assert.True(dataStore.ClassroomTypeDeleted);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTypeDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomTypeDataStore(null);
        var useCase = new SoftDeleteClassroomTypeUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomTypeNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}