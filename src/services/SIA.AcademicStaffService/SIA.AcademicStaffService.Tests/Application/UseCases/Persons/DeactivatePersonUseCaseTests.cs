using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Persons;

public sealed class DeactivatePersonUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingPerson_ShouldDeactivate()
    {
        var person = new Person(Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "Maestría", "ana@example.com", "7821234567");

        var dataStore = new FakePersonDataStore { PersonById = person };
        var useCase = new DeactivatePersonUseCase(dataStore);

        await useCase.ExecuteAsync(person.TenantId, person.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(person.Status);
        Assert.True(dataStore.PersonDeactivated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakePersonDataStore { PersonById = null };
        var useCase = new DeactivatePersonUseCase(dataStore);

        await Assert.ThrowsAsync<PersonNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.PersonDeactivated);
    }
}