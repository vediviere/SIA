using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface IPersonDataStore
{
    Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken);

    Task AddPersonWithOutboxAsync(Person person, PersonCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<Person?> GetPersonByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task UpdatePersonWithOutboxAsync(Person person, PersonUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task ActivatePersonWithOutboxAsync(Person person, PersonActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivatePersonWithOutboxAsync(Person person, PersonDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}