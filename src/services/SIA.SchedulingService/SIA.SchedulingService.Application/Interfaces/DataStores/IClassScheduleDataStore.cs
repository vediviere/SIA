using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IClassScheduleDataStore
{
    Task<ClassSchedule?> GetClassScheduleByIdAsync(Guid tenantId, Guid classScheduleId, CancellationToken cancellationToken);

    Task AddClassScheduleWithOutboxAsync(ClassSchedule classSchedule,ClassScheduleCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task UpdateClassScheduleWithOutboxAsync(ClassSchedule classSchedule,ClassScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task SoftDeleteClassScheduleWithOutboxAsync(ClassSchedule classSchedule,ClassScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreClassScheduleWithOutboxAsync(ClassSchedule classSchedule,ClassScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}