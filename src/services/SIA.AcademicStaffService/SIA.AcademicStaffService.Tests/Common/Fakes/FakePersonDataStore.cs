using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakePersonDataStore : IPersonDataStore
{
    public Person? PersonById { get; set; }
    public bool EmployeeNumberExistsResult { get; set; }

    public bool PersonAdded { get; private set; }
    public bool PersonUpdated { get; private set; }
    public bool PersonActivated { get; private set; }
    public bool PersonDeactivated { get; private set; }

    public Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken)
        => Task.FromResult(EmployeeNumberExistsResult);

    public Task<Person?> GetPersonByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonById);

    public Task AddPersonWithOutboxAsync(Person person, PersonCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        PersonAdded = true;
        return Task.CompletedTask;
    }

    public Task UpdatePersonWithOutboxAsync(Person person, PersonUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        PersonUpdated = true;
        return Task.CompletedTask;
    }

    public Task ActivatePersonWithOutboxAsync(Person person, PersonActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        PersonActivated = true;
        return Task.CompletedTask;
    }

    public Task DeactivatePersonWithOutboxAsync(Person person, PersonDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        PersonDeactivated = true;
        return Task.CompletedTask;
    }
}