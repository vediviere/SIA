using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface IAcademicPeriodsDataStore
{
    Task<bool> AcademicPeriodCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);
    Task AddAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task DeactivateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task ActivateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);


}