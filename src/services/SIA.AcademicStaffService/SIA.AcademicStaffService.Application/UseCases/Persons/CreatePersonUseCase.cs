using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Contracts.Requests.Persons;
using SIA.AcademicStaffService.Contracts.Responses.Persons;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.Persons;

public sealed class CreatePersonUseCase
{
    private readonly IPersonDataStore _dataStore;

    public CreatePersonUseCase(IPersonDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreatePersonResponse> ExecuteAsync(
        CreatePersonRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var normalizedEmployeeNumber = request.EmployeeNumber.Trim();

        var employeeNumberExists = await _dataStore.EmployeeNumberExistsAsync(
            normalizedEmployeeNumber,
            cancellationToken);

        if (employeeNumberExists)
        {
            throw new DuplicatePersonEmployeeNumberException(normalizedEmployeeNumber);
        }

        var person = new Person(
            request.TenantId,
            normalizedEmployeeNumber,
            request.FirstName,
            request.PaternalLastName,
            request.MaternalLastName ?? string.Empty,
            request.Email,
            request.Phone ?? string.Empty);

        var integrationEvent = new PersonCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = person.CreatedAtUtc,
            TenantId = person.TenantId,
            PersonId = person.Id,
            EmployeeNumber = person.EmployeeNumber,
            FirstName = person.FirstName,
            PaternalLastName = person.PaternalLastName,
            MaternalLastName = person.MaternalLastName,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status,
            Version = 1
        };

        await _dataStore.AddPersonWithOutboxAsync(person, integrationEvent, cancellationToken);

        return new CreatePersonResponse
        {
            Id = person.Id,
            TenantId = person.TenantId,
            EmployeeNumber = person.EmployeeNumber,
            FirstName = person.FirstName,
            PaternalLastName = person.PaternalLastName,
            MaternalLastName = person.MaternalLastName,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status,
            CreatedAtUtc = person.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}