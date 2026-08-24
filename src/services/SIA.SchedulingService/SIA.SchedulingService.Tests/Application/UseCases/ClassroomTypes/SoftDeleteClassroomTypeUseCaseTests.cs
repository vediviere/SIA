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
        var correlationId = Guid.NewGuid();
        var existingType = new ClassroomType(tenantId, "LAB", "Lab", "Desc");
        var dataStore = new FakeClassroomTypeDataStore(existingType);
        var useCase = new SoftDeleteClassroomTypeUseCase(dataStore);

        var response = await useCase.ExecuteAsync(tenantId, existingType.Id, correlationId, CancellationToken.None);

        Assert.False(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.DeletedType);
        Assert.False(dataStore.DeletedType.Status);
        Assert.NotNull(dataStore.DeletedEvent);
        Assert.Equal(existingType.Id, dataStore.DeletedEvent.ClassroomTypeId);
        Assert.Equal(correlationId, dataStore.DeletedEvent.CorrelationId);
        Assert.Equal(1, dataStore.DeletedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTypeDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomTypeDataStore(null);
        var useCase = new SoftDeleteClassroomTypeUseCase(dataStore);

        await Assert.ThrowsAsync<ClassroomTypeNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeletedType);
        Assert.Null(dataStore.DeletedEvent);
    }
}