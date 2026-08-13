using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;
public interface IAcademicLoadDataStore
{
    Task AddAcademicLoadWithOutboxAsync(AcademicLoad academicLoad, AcademicLoadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task<AcademicLoad?> GetByIdAsync(Guid tenantId, Guid AcademicLoadId,  CancellationToken cancellationToken);
    Task UpdateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task DeactivateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task ActivateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}