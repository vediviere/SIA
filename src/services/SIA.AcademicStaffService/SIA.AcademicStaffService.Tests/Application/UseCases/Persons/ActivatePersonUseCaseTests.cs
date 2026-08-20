using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Persons;

public sealed class ActivatePersonUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingPerson_ShouldActivate()
    {
        var person = new Person(Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "Maestría", "ana@example.com", "7821234567");
        person.Deactivate();

        var dataStore = new FakePersonDataStore { PersonById = person };
        var useCase = new ActivatePersonUseCase(dataStore);

        await useCase.ExecuteAsync(person.TenantId, person.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.True(person.Status);
        Assert.True(dataStore.PersonActivated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakePersonDataStore { PersonById = null };
        var useCase = new ActivatePersonUseCase(dataStore);

        await Assert.ThrowsAsync<PersonNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.PersonActivated);
    }
}