using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.UseCases.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Tests.Common.Fakes;

namespace SIA.AcademicStaffService.Tests.Application.UseCases.DivisionManagers;

public sealed class ActivateDivisionHeadUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingDivisionHead_ShouldActivate()
    {
        var divisionHead = new DivisionHead(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        divisionHead.Deactivate();

        var dataStore = new FakeDivisionHeadDataStore { DivisionHeadById = divisionHead };
        var useCase = new ActivateDivisionHeadUseCase(dataStore);

        await useCase.ExecuteAsync(divisionHead.TenantId, divisionHead.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.True(divisionHead.Status);
        Assert.True(dataStore.DivisionHeadActivated);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDivisionHeadNotFound_ShouldThrowNotFound()
    {
        var dataStore = new FakeDivisionHeadDataStore { DivisionHeadById = null };
        var useCase = new ActivateDivisionHeadUseCase(dataStore);

        await Assert.ThrowsAsync<DivisionHeadNotFoundException>(() => useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

        Assert.False(dataStore.DivisionHeadActivated);
    }
}