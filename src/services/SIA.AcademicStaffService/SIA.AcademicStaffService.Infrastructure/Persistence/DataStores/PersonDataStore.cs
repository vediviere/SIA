using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.DataStores;

public sealed class PersonDataStore : IPersonDataStore
{
    private readonly AcademicStaffDbContext _dbContext;

    public PersonDataStore(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken)
    {
        return _dbContext.Persons.AnyAsync(
            person => person.EmployeeNumber == employeeNumber,
            cancellationToken);
    }

    public Task<Person?> GetPersonByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
    {
        return _dbContext.Persons.FirstOrDefaultAsync(
            person => person.TenantId == tenantId && person.Id == personId,
            cancellationToken);
    }

    public async Task AddPersonWithOutboxAsync(Person person, PersonCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.PersonCreatedV1, payload, integrationEvent.CorrelationId);

        await _dbContext.Persons.AddAsync(person, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePersonWithOutboxAsync(Person person, PersonUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.PersonUpdatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Persons.Update(person);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivatePersonWithOutboxAsync(Person person, PersonActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.PersonActivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Persons.Update(person);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivatePersonWithOutboxAsync(Person person, PersonDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.PersonDeactivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Persons.Update(person);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}