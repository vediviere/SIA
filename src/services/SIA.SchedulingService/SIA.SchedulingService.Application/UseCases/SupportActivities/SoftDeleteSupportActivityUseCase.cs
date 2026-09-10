using SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Contracts.Responses.SupportActivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.SupportActivities;

public sealed class SoftDeleteSupportActivityUseCase
{
    private readonly ISupportActivityDataStore _dataStore;

    public SoftDeleteSupportActivityUseCase(ISupportActivityDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid supportActivityId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportActivity = await _dataStore.GetSupportActivityByIdAsync(tenantId, supportActivityId, cancellationToken);

        if (supportActivity is null)
        {
            throw new SupportActivityNotFoundException(supportActivityId);
        }

        supportActivity.SoftDelete();

        var integrationEvent = new SupportActivityDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportActivity.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = supportActivity.TenantId,
            SupportActivityId = supportActivity.Id,
            Status = supportActivity.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteSupportActivityWithOutboxAsync(supportActivity, integrationEvent, cancellationToken);
    }
}