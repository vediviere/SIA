using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Contracts.Requests.Coordinators;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Coordinators;

public sealed class CreateCoordinatorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateCoordinator()
    {
        var tenantId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeCoordinatorDataStore();
        var useCase = new CreateCoordinatorUseCase(dataStore);

        var response = await useCase.ExecuteAsync(new CreateCoordinatorRequest
        {
            TenantId = tenantId,
            PersonId = personId
        }, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(personId, response.PersonId);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.CoordinatorAdded);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonAlreadyCoordinator_ShouldThrowConflict()
    {
        var dataStore = new FakeCoordinatorDataStore { PersonAlreadyCoordinatorResult = true };
        var useCase = new CreateCoordinatorUseCase(dataStore);

        await Assert.ThrowsAsync<DuplicateCoordinatorException>(() => useCase.ExecuteAsync(new CreateCoordinatorRequest
        {
            TenantId = Guid.NewGuid(),
            PersonId = Guid.NewGuid()
        }, Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.CoordinatorAdded);
    }
}