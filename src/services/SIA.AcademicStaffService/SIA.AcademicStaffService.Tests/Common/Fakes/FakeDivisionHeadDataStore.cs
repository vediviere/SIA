using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionHeads;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeDivisionHeadDataStore : IDivisionHeadDataStore
{
    public DivisionHead? DivisionHeadById { get; set; }
    public bool PersonAlreadyManagesProgramResult { get; set; }

    public DivisionHead? AddedDivisionHead { get; private set; }
    public DivisionHeadCreatedIntegrationEvent? AddedEvent { get; private set; }

    public DivisionHead? ActivatedDivisionHead { get; private set; }
    public DivisionHeadActivatedIntegrationEvent? ActivatedEvent { get; private set; }

    public DivisionHead? DeactivatedDivisionHead { get; private set; }
    public DivisionHeadDeactivatedIntegrationEvent? DeactivatedEvent { get; private set; }

    public Task<bool> PersonAlreadyManagesProgramAsync(Guid tenantId, Guid programId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonAlreadyManagesProgramResult);

    public Task<DivisionHead?> GetDivisionHeadByIdAsync(Guid tenantId, Guid divisionHeadId, CancellationToken cancellationToken)
        => Task.FromResult(DivisionHeadById);

    public Task AddDivisionHeadWithOutboxAsync(DivisionHead divisionHead, DivisionHeadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedDivisionHead = divisionHead;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ActivateDivisionHeadWithOutboxAsync(DivisionHead divisionHead, DivisionHeadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivatedDivisionHead = divisionHead;
        ActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task DeactivateDivisionHeadWithOutboxAsync(DivisionHead divisionHead, DivisionHeadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeactivatedDivisionHead = divisionHead;
        DeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}