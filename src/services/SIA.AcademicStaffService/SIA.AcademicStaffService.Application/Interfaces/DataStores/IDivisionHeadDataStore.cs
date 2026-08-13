using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface IDivisionHeadDataStore
{
    Task<bool> PersonAlreadyManagesProgramAsync(Guid tenantId, Guid programId, Guid personId, CancellationToken cancellationToken);

    Task AddDivisionManagerWithOutboxAsync(DivisionHead divisionManager, DivisionHeadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<DivisionHead?> GetDivisionManagerByIdAsync(Guid tenantId, Guid divisionManagerId, CancellationToken cancellationToken);

    Task UpdateDivisionManagerWithOutboxAsync(DivisionHead divisionManager, DivisionHeadUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task ActivateDivisionManagerWithOutboxAsync(DivisionHead divisionManager, DivisionHeadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateDivisionManagerWithOutboxAsync(DivisionHead divisionManager, DivisionHeadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}