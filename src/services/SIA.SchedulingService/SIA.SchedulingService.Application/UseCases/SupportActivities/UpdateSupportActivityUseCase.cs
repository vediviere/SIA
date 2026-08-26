using SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Contracts.Requests.SupportActivity;
using SIA.SchedulingService.Contracts.Responses.SupportActivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.SupportActivities;

public sealed class UpdateSupportActivityUseCase
{
    private readonly ISupportActivityDataStore _dataStore;

    public UpdateSupportActivityUseCase(ISupportActivityDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateSupportActivityResponse> ExecuteAsync(
        Guid tenantId,
        Guid supportActivityId,
        UpdateSupportActivityRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportActivity = await _dataStore.GetSupportActivityByIdAsync(tenantId, supportActivityId, cancellationToken);

        if (supportActivity is null)
        {
            throw new SupportActivityNotFoundException(supportActivityId);
        }

        supportActivity.Update(
            request.Activity,
            request.Observation);

        var integrationEvent = new SupportActivityUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportActivity.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = supportActivity.TenantId,
            SupportActivityId = supportActivity.Id,
            Activity = supportActivity.Activity,
            Observation = supportActivity.Observation,
            Status = supportActivity.Status,
            Version = 1
        };

        await _dataStore.UpdateSupportActivityWithOutboxAsync(supportActivity, integrationEvent, cancellationToken);

        return new UpdateSupportActivityResponse
        {
            Id = supportActivity.Id,
            TenantId = supportActivity.TenantId,
            Activity = supportActivity.Activity,
            Observation = supportActivity.Observation,
            Status = supportActivity.Status,
            UpdatedAtUtc = supportActivity.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}