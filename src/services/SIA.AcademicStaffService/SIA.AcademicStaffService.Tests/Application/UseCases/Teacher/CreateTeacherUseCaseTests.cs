using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Contracts.Requests.Professors;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.Professors;

public sealed class CreateTeacherUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateTeacher()
    {
        var tenantId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeTeacherDataStore();
        var useCase = new CreateTeacherUseCase(dataStore);

        var response = await useCase.ExecuteAsync(new CreateTeacherRequest
        {
            TenantId = tenantId,
            PersonId = personId,
            ProfessionalProfile = "Ingeniero de Software",
            ContractType = "Tiempo completo",
            ContractHours = 40
        }, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(personId, response.PersonId);
        Assert.Equal(40, response.ContractHours);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(dataStore.AddedTeacher);
        Assert.Equal(personId, dataStore.AddedTeacher.PersonId);
        Assert.NotNull(dataStore.AddedEvent);
        Assert.Equal(dataStore.AddedTeacher.Id, dataStore.AddedEvent.ProfessorId);
        Assert.Equal(tenantId, dataStore.AddedEvent.TenantId);
        Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
        Assert.Equal(1, dataStore.AddedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonAlreadyProfessor_ShouldThrowConflict()
    {
        var dataStore = new FakeTeacherDataStore { PersonAlreadyProfessorResult = true };
        var useCase = new CreateTeacherUseCase(dataStore);

        await Assert.ThrowsAsync<DuplicateTeacherException>(() => useCase.ExecuteAsync(new CreateTeacherRequest
        {
            TenantId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            ProfessionalProfile = "Ingeniero de Software",
            ContractType = "Tiempo completo",
            ContractHours = 40
        }, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedTeacher);
        Assert.Null(dataStore.AddedEvent);
    }
}