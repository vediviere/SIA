using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;

namespace SIA.AcademicStaffService.Application.UseCases.Persons;

public sealed class DeactivatePersonUseCase
{
    private readonly IPersonDataStore _dataStore;

    public DeactivatePersonUseCase(IPersonDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid personId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var person = await _dataStore.GetPersonByIdAsync(tenantId, personId, cancellationToken);

        if (person is null)
        {
            throw new PersonNotFoundException(personId);
        }

        person.Deactivate();

        var integrationEvent = new PersonDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = person.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = person.TenantId,
            PersonId = person.Id,
            Version = 1
        };

        await _dataStore.DeactivatePersonWithOutboxAsync(person, integrationEvent, cancellationToken);
    }
}