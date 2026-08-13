using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.Responses.ClassSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.ClassSchedules;

public sealed class RestoreClassScheduleUseCase
{
    private readonly IClassScheduleDataStore _dataStore;

    public RestoreClassScheduleUseCase(IClassScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<RestoreClassScheduleResponse> ExecuteAsync(
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

        classSchedule.Restore();

        var integrationEvent = new ClassScheduleRestoredIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classSchedule.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classSchedule.TenantId,
            ClassScheduleId = classSchedule.Id,
            Status = classSchedule.Status,
            Version = 1
        };

        await _dataStore.RestoreClassScheduleWithOutboxAsync(classSchedule, integrationEvent, cancellationToken);

        return new RestoreClassScheduleResponse
        {
            Id = classSchedule.Id,
            Status = classSchedule.Status,
            UpdatedAtUtc = classSchedule.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}