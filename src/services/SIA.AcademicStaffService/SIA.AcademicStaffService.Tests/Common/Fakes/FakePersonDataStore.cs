using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakePersonDataStore : IPersonDataStore
{
    public Person? PersonById { get; set; }
    public bool EmployeeNumberExistsResult { get; set; }

    public Person? AddedPerson { get; private set; }
    public PersonCreatedIntegrationEvent? AddedEvent { get; private set; }

    public Person? UpdatedPerson { get; private set; }
    public PersonUpdatedIntegrationEvent? UpdatedEvent { get; private set; }

    public Person? ActivatedPerson { get; private set; }
    public PersonActivatedIntegrationEvent? ActivatedEvent { get; private set; }

    public Person? DeactivatedPerson { get; private set; }
    public PersonDeactivatedIntegrationEvent? DeactivatedEvent { get; private set; }

    public Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken)
        => Task.FromResult(EmployeeNumberExistsResult);

    public Task<Person?> GetPersonByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonById);

    public Task AddPersonWithOutboxAsync(Person person, PersonCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedPerson = person;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdatePersonWithOutboxAsync(Person person, PersonUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedPerson = person;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ActivatePersonWithOutboxAsync(Person person, PersonActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivatedPerson = person;
        ActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task DeactivatePersonWithOutboxAsync(Person person, PersonDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeactivatedPerson = person;
        DeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}