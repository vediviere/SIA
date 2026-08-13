using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IAcademicOfferingDataStore
{
    Task<bool> ExistsByGroupAndSubjectAsync(Guid groupId, Guid subjectId, CancellationToken cancellationToken);
    Task AddAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingCreatedIntegrationEvet integrationEvent, CancellationToken cancellationToken);
    Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken);
    Task UpdateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task DeactivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task ActivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}