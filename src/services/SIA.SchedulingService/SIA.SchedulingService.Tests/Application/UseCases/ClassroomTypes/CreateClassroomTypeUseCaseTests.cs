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
        var dataStore = new FakeClassroomTypeDataStore();
        var useCase = new CreateClassroomTypeUseCase(dataStore);

        var request = new CreateClassroomTypeRequest
        {
            TenantId = Guid.NewGuid(),
            Code = "LAB-COMP",
            Name = "Laboratorio de Cómputo",
            Description = "Desc"
        };

        var response = await useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("LAB-COMP", response.Code);
        Assert.True(dataStore.ClassroomTypeAdded);
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
    }
}