using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IAcademicOfferingDataStore
{
  Task<bool> ExistsByGroupAndSubjectAsync(Guid tenantId, Guid groupId, Guid subjectId, CancellationToken cancellationToken);
  Task AddAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingCreatedIntegrationEvet integrationEvent, CancellationToken cancellationToken);
  Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken);
  Task UpdateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
  Task DeactivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
  Task ActivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
