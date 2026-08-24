using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Contracts.Requests.ClassroomType;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.ClassroomTypes;

public sealed class CreateClassroomTypeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateClassroomType()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeClassroomTypeDataStore();
        var useCase = new CreateClassroomTypeUseCase(dataStore);

        var request = new CreateClassroomTypeRequest
        {
            TenantId = tenantId,
            Code = "LAB-COMP",
            Name = "Laboratorio de Cómputo",
            Description = "Desc"
        };

        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedType);
        Assert.Equal("LAB-COMP", dataStore.AddedType.Code);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(response.Id, dataStore.AddedEvent.ClassroomTypeId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNameAlreadyExists_ShouldThrowDuplicateClassroomTypeNameException()
    {
        var dataStore = new FakeClassroomTypeDataStore { NameExistsResult = true };
        var useCase = new CreateClassroomTypeUseCase(dataStore);

        var request = new CreateClassroomTypeRequest
        {
            TenantId = Guid.NewGuid(),
            Code = "LAB",
            Name = "Lab Existente",
            Description = "Desc"
        };

        await Assert.ThrowsAsync<DuplicateClassroomTypeNameException>(() =>
            useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedType);
        Assert.Null(dataStore.AddedEvent);
    }
}