using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeDivisionHeadDataStore : IDivisionHeadDataStore
{
    public DivisionHead? DivisionHeadById { get; set; }
    public bool PersonAlreadyManagesProgramResult { get; set; }

    public bool DivisionHeadAdded { get; private set; }
    public bool DivisionHeadActivated { get; private set; }
    public bool DivisionHeadDeactivated { get; private set; }

    public Task<bool> PersonAlreadyManagesProgramAsync(Guid tenantId, Guid programId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonAlreadyManagesProgramResult);

    public Task<DivisionHead?> GetDivisionManagerByIdAsync(Guid tenantId, Guid divisionManagerId, CancellationToken cancellationToken)
        => Task.FromResult(DivisionHeadById);

    public Task AddDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DivisionHeadAdded = true;
        return Task.CompletedTask;
    }

    public Task ActivateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DivisionHeadActivated = true;
        return Task.CompletedTask;
    }

    public Task DeactivateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DivisionHeadDeactivated = true;
        return Task.CompletedTask;
    }
}