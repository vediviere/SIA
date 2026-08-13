using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;

namespace SIA.SchedulingService.Application.Interfaces.DataStores
{
    public interface ISupportScheduleDataStore
    {
        Task<SupportSchedule?> GetSupportScheduleByIdAsync(Guid tenantId, Guid supportScheduleId, CancellationToken cancellationToken);

        Task AddSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task UpdateSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task SoftDeleteSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task RestoreSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}
