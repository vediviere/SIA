using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Contracts.Requests.SupportActivity;
using SIA.SchedulingService.Contracts.Responses.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.SupportActivities;

public sealed class CreateSupportActivityUseCase
{
    private readonly ISupportActivityDataStore _dataStore;

    public CreateSupportActivityUseCase(ISupportActivityDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateSupportActivityResponse> ExecuteAsync(
        CreateSupportActivityRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportActivity = new SupportActivity(
            request.TenantId,
            request.Activity,
            request.Observation);

        var integrationEvent = new SupportActivityCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportActivity.CreatedAtUtc,
            TenantId = supportActivity.TenantId,
            SupportActivityId = supportActivity.Id,
            Activity = supportActivity.Activity,
            Observation = supportActivity.Observation,
            Status = supportActivity.Status,
            Version = 1
        };

        await _dataStore.AddSupportActivityWithOutboxAsync(supportActivity, integrationEvent, cancellationToken);

        return new CreateSupportActivityResponse
        {
            Id = supportActivity.Id,
            TenantId = supportActivity.TenantId,
            Activity = supportActivity.Activity,
            Observation = supportActivity.Observation,
            Status = supportActivity.Status,
            CreatedAtUtc = supportActivity.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}