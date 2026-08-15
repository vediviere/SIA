using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Contracts.Requests.Persons;
using SIA.AcademicStaffService.Contracts.Responses.Persons;

namespace SIA.AcademicStaffService.Application.UseCases.Persons;

public sealed class UpdatePersonUseCase
{
    private readonly IPersonDataStore _dataStore;

    public UpdatePersonUseCase(IPersonDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdatePersonResponse> ExecuteAsync(
        Guid tenantId,
        Guid personId,
        UpdatePersonRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var person = await _dataStore.GetPersonByIdAsync(tenantId, personId, cancellationToken);

        if (person is null)
        {
            throw new PersonNotFoundException(personId);
        }

        person.Update(
            request.FirstName,
            request.PaternalLastName,
            request.MaternalLastName,
            request.AcademicDegree,
            request.Email,
            request.Phone);

        var integrationEvent = new PersonUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = person.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = person.TenantId,
            PersonId = person.Id,
            EmployeeNumber = person.EmployeeNumber,
            FirstName = person.FirstName,
            PaternalLastName = person.PaternalLastName,
            MaternalLastName = person.MaternalLastName,
            AcademicDegree = person.AcademicDegree,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status,
            Version = 1
        };

        await _dataStore.UpdatePersonWithOutboxAsync(person, integrationEvent, cancellationToken);

        return new UpdatePersonResponse
        {
            Id = person.Id,
            TenantId = person.TenantId,
            EmployeeNumber = person.EmployeeNumber,
            FirstName = person.FirstName,
            PaternalLastName = person.PaternalLastName,
            MaternalLastName = person.MaternalLastName,
            AcademicDegree = person.AcademicDegree,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status,
            UpdatedAtUtc = person.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}