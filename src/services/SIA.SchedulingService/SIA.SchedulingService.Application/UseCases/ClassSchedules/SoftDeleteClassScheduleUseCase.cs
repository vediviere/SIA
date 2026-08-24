using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.Responses.ClassSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.ClassSchedules;

public sealed class SoftDeleteClassScheduleUseCase
{
    private readonly IClassScheduleDataStore _dataStore;

    public SoftDeleteClassScheduleUseCase(IClassScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid classScheduleId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classSchedule = await _dataStore.GetClassScheduleByIdAsync(tenantId, classScheduleId, cancellationToken);

        if (classSchedule is null)
        {
            throw new ClassScheduleNotFoundException(classScheduleId);
        }

        classSchedule.SoftDelete();

        var integrationEvent = new ClassScheduleDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classSchedule.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classSchedule.TenantId,
            ClassScheduleId = classSchedule.Id,
            Status = classSchedule.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteClassScheduleWithOutboxAsync(classSchedule, integrationEvent, cancellationToken);
    }
}