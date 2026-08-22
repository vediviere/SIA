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
        var correlationId = Guid.NewGuid();

        var dataStore = new FakePersonDataStore { PersonById = person };
        var useCase = new ActivatePersonUseCase(dataStore);

        await useCase.ExecuteAsync(person.TenantId, person.Id, correlationId, CancellationToken.None);

        Assert.True(person.Status);
        Assert.NotNull(dataStore.ActivatedPerson);
        Assert.Equal(person.Id, dataStore.ActivatedPerson.Id);

        Assert.NotNull(dataStore.ActivatedEvent);
        Assert.Equal(person.Id, dataStore.ActivatedEvent.PersonId);
        Assert.Equal(person.TenantId, dataStore.ActivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.ActivatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakePersonDataStore { PersonById = null };
        var useCase = new ActivatePersonUseCase(dataStore);

        await Assert.ThrowsAsync<PersonNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.ActivatedPerson);
        Assert.Null(dataStore.ActivatedEvent);
    }
}