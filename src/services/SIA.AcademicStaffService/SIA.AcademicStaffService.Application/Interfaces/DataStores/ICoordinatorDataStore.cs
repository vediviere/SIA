using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.DataStores;

public interface ICoordinatorDataStore
{
    Task<bool> PersonAlreadyCoordinatorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task AddCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<Coordinator?> GetCoordinatorByIdAsync(Guid tenantId, Guid coordinatorId, CancellationToken cancellationToken);

    Task ActivateCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task DeactivateCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}