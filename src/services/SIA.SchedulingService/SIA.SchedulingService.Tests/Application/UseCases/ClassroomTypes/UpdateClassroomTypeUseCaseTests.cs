using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Contracts.Requests.ClassroomType;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassroomTypes;

public sealed class UpdateClassroomTypeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateClassroomType()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var existingType = new ClassroomType(tenantId, "LAB", "Lab", "Desc");
        var dataStore = new FakeClassroomTypeDataStore(existingType);
        var useCase = new UpdateClassroomTypeUseCase(dataStore);

        var request = new UpdateClassroomTypeRequest
        {
            Code = "LAB-NEW",
            Name = "Lab Actualizado",
            Description = "Desc Actualizada"
        };

        var response = await useCase.ExecuteAsync(tenantId, existingType.Id, request, correlationId, CancellationToken.None);

        Assert.Equal("LAB-NEW", response.Code);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.UpdatedType);
        Assert.Equal("LAB-NEW", dataStore.UpdatedType.Code);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(existingType.Id, dataStore.UpdatedEvent.ClassroomTypeId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
        Assert.Equal(1, dataStore.UpdatedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTypeDoesNotExist_ShouldThrowNotFoundException()
    {
        var dataStore = new FakeClassroomTypeDataStore(null);
        var useCase = new UpdateClassroomTypeUseCase(dataStore);
        var request = new UpdateClassroomTypeRequest { Code = "L", Name = "N", Description = "D" };

        await Assert.ThrowsAsync<ClassroomTypeNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedType);
        Assert.Null(dataStore.UpdatedEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNameChangesAndAlreadyExists_ShouldThrowDuplicateException()
    {
        var tenantId = Guid.NewGuid();
        var existingType = new ClassroomType(tenantId, "LAB", "Viejo Nombre", "Desc");
        var dataStore = new FakeClassroomTypeDataStore(existingType) { NameExistsResult = true };
        var useCase = new UpdateClassroomTypeUseCase(dataStore);

        var request = new UpdateClassroomTypeRequest { Code = "LAB", Name = "Nuevo Nombre", Description = "Desc" };

        await Assert.ThrowsAsync<DuplicateClassroomTypeNameException>(() =>
            useCase.ExecuteAsync(tenantId, existingType.Id, request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.UpdatedType);
        Assert.Null(dataStore.UpdatedEvent);
    }
}