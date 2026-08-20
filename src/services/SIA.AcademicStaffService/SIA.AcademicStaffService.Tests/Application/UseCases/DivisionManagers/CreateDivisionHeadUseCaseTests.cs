using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Requests.DivisionManagers;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.DivisionManagers;

public sealed class CreateDivisionHeadUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateDivisionHead()
    {
        var tenantId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var dataStore = new FakeDivisionHeadDataStore();
        var useCase = new CreateDivisionHeadUseCase(dataStore);

        var response = await useCase.ExecuteAsync(new CreateDivisionHeadRequest
        {
            TenantId = tenantId,
            ProgramId = programId,
            PersonId = personId
        }, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(programId, response.ProgramId);
        Assert.Equal(personId, response.PersonId);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(dataStore.DivisionHeadAdded);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPersonAlreadyManagesProgram_ShouldThrowConflict()
    {
        var dataStore = new FakeDivisionHeadDataStore { PersonAlreadyManagesProgramResult = true };
        var useCase = new CreateDivisionHeadUseCase(dataStore);

        await Assert.ThrowsAsync<DuplicateDivisionHeadException>(() => useCase.ExecuteAsync(new CreateDivisionHeadRequest
        {
            TenantId = Guid.NewGuid(),
            ProgramId = Guid.NewGuid(),
            PersonId = Guid.NewGuid()
        }, Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.DivisionHeadAdded);
    }
}