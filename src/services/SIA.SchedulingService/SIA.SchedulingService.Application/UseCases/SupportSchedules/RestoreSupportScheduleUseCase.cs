using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Contracts.Responses.SupportSchedules;
using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.SupportSchedules;

public sealed class RestoreSupportScheduleUseCase
{
    private readonly ISupportScheduleDataStore _dataStore;

    public RestoreSupportScheduleUseCase(ISupportScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid supportScheduleId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportSchedule = await _dataStore.GetSupportScheduleByIdAsync(tenantId, supportScheduleId, cancellationToken);

        if (supportSchedule is null)
        {
            throw new SupportScheduleNotFoundException(supportScheduleId);
        }

        supportSchedule.Restore();

        var integrationEvent = new SupportScheduleRestoredIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportSchedule.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = supportSchedule.TenantId,
            SupportScheduleId = supportSchedule.Id,
            Status = supportSchedule.Status,
            Version = 1
        };

        await _dataStore.RestoreSupportScheduleWithOutboxAsync(supportSchedule, integrationEvent, cancellationToken);
    }
}