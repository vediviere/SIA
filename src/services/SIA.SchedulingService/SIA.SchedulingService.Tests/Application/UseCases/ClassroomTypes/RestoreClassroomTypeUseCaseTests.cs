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
        var correlationId = Guid.NewGuid();
        var existingType = new ClassroomType(tenantId, "LAB", "Lab", "Desc");
        existingType.SoftDelete();

        var dataStore = new FakeClassroomTypeDataStore(existingType);
        var useCase = new RestoreClassroomTypeUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingType.Id, correlationId, CancellationToken.None);

        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.RestoredType);
        Assert.True(dataStore.RestoredType.Status);
        Assert.NotNull(dataStore.RestoredEvent);
        Assert.Equal(existingType.Id, dataStore.RestoredEvent.ClassroomTypeId);
        Assert.Equal(correlationId, dataStore.RestoredEvent.CorrelationId);
        Assert.Equal(1, dataStore.RestoredEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTypeDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomTypeDataStore(null);
        var useCase = new RestoreClassroomTypeUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomTypeNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.RestoredType);
        Assert.Null(dataStore.RestoredEvent);
    }
}