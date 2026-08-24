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
        var correlationId = Guid.NewGuid();
        var dataStore = new FakePersonDataStore { PersonById = person };
        var useCase = new DeactivatePersonUseCase(dataStore);

        await useCase.ExecuteAsync(person.TenantId, person.Id, correlationId, CancellationToken.None);

        Assert.False(person.Status);
        Assert.NotNull(dataStore.DeactivatedPerson);
        Assert.Equal(person.Id, dataStore.DeactivatedPerson.Id);
        Assert.NotNull(dataStore.DeactivatedEvent);
        Assert.Equal(person.Id, dataStore.DeactivatedEvent.PersonId);
        Assert.Equal(person.TenantId, dataStore.DeactivatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.DeactivatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakePersonDataStore { PersonById = null };
        var useCase = new DeactivatePersonUseCase(dataStore);

        await Assert.ThrowsAsync<PersonNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.DeactivatedPerson);
        Assert.Null(dataStore.DeactivatedEvent);
    }
}