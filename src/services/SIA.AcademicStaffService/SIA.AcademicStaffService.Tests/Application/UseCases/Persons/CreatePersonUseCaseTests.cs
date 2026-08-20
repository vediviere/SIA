using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Contracts.Requests.Persons;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Persons;

public sealed class CreatePersonUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreatePerson()
    {
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakePersonDataStore();
        var useCase = new CreatePersonUseCase(dataStore);

        var response = await useCase.ExecuteAsync(new CreatePersonRequest
        {
            TenantId = tenantId,
            EmployeeNumber = " EMP-0001 ",
            FirstName = "Ana",
            PaternalLastName = "García",
            MaternalLastName = "López",
            AcademicDegree = "Maestría",
            Email = "ana@example.com",
            Phone = "7821234567"
        }, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("EMP-0001", response.EmployeeNumber);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.PersonAdded);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmployeeNumberExists_ShouldThrowConflict()
    {
        var dataStore = new FakePersonDataStore { EmployeeNumberExistsResult = true };
        var useCase = new CreatePersonUseCase(dataStore);

        await Assert.ThrowsAsync<DuplicatePersonEmployeeNumberException>(() => useCase.ExecuteAsync(new CreatePersonRequest
        {
            TenantId = Guid.NewGuid(),
            EmployeeNumber = "EMP-0001",
            FirstName = "Ana",
            PaternalLastName = "García",
            MaternalLastName = "López",
            AcademicDegree = "Maestría",
            Email = "ana@example.com",
            Phone = "7821234567"
        }, Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.PersonAdded);
    }
}