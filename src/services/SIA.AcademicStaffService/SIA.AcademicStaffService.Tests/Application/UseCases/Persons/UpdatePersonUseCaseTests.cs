using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Contracts.Requests.Persons;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Persons;

public sealed class UpdatePersonUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdatePerson()
    {
        var person = new Person(Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "Maestría", "ana@example.com", "7821234567");
        var correlationId = Guid.NewGuid();
        var dataStore = new FakePersonDataStore { PersonById = person };
        var useCase = new UpdatePersonUseCase(dataStore);

        var response = await useCase.ExecuteAsync(
            person.TenantId,
            person.Id,
            new UpdatePersonRequest
            {
                FirstName = "Ana María",
                PaternalLastName = "García",
                MaternalLastName = "López",
                AcademicDegree = "Doctorado",
                Email = "nueva@example.com",
                Phone = "7820000000"
            },
            correlationId,
            CancellationToken.None);

        Assert.Equal("Ana María", response.FirstName);
        Assert.Equal("Doctorado", response.AcademicDegree);
        Assert.Equal("nueva@example.com", response.Email);
        Assert.NotNull(dataStore.UpdatedPerson);
        Assert.Equal("Doctorado", dataStore.UpdatedPerson.AcademicDegree);
        Assert.NotNull(dataStore.UpdatedEvent);
        Assert.Equal(person.Id, dataStore.UpdatedEvent.PersonId);
        Assert.Equal(person.TenantId, dataStore.UpdatedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.UpdatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakePersonDataStore { PersonById = null };
        var useCase = new UpdatePersonUseCase(dataStore);

        await Assert.ThrowsAsync<PersonNotFoundException>(() => useCase.ExecuteAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdatePersonRequest
            {
                FirstName = "Ana",
                PaternalLastName = "García",
                MaternalLastName = "López",
                AcademicDegree = "Maestría",
                Email = "ana@example.com",
                Phone = "7821234567"
            },
            Guid.NewGuid(),
            CancellationToken.None));

        Assert.Null(dataStore.UpdatedPerson);
        Assert.Null(dataStore.UpdatedEvent);
    }
}