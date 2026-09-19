using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionHeads;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface IDivisionHeadDataStore
{
    Task<bool> PersonAlreadyManagesProgramAsync(Guid tenantId, Guid programId, Guid personId, CancellationToken cancellationToken);

    Task AddDivisionHeadWithOutboxAsync(DivisionHead divisionHead, DivisionHeadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<DivisionHead?> GetDivisionHeadByIdAsync(Guid tenantId, Guid divisionHeadId, CancellationToken cancellationToken);

    Task ActivateDivisionHeadWithOutboxAsync(DivisionHead divisionHead, DivisionHeadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateDivisionHeadWithOutboxAsync(DivisionHead divisionHead, DivisionHeadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}