using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Tests.Common.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Application.UseCases.Classrooms;

public sealed class CreateClassroomLabUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateClassroomLab()
    {
        var dataStore = new FakeClassroomLabDataStore();
        var useCase = new CreateClassroomLabUseCase(dataStore);

        var request = new CreateClassroomLabRequest
        {
            TenantId = Guid.NewGuid(),
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Laboratorio Redes",
            Capacity = 30,
            Description = "Desc"
        };

        var response = await useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("LAB-01", response.Code);
        Assert.True(dataStore.ClassroomLabAdded);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateClassroomLabCodeException()
    {
        var dataStore = new FakeClassroomLabDataStore { CodeExistsResult = true };
        var useCase = new CreateClassroomLabUseCase(dataStore);

        var request = new CreateClassroomLabRequest
        {
            TenantId = Guid.NewGuid(),
            BuildingId = Guid.NewGuid(),
            ClassroomTypeId = Guid.NewGuid(),
            Code = "LAB-01",
            Name = "Lab",
            Capacity = 30
        };

        await Assert.ThrowsAsync<DuplicateClassroomLabCodeException>(() =>
            useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));
    }
}