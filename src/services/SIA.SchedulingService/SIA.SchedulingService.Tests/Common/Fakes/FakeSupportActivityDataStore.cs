using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeSupportActivityDataStore : ISupportActivityDataStore
{
    private readonly SupportActivity? _existingActivity;

    public SupportActivity? AddedActivity { get; private set; }
    public SupportActivityCreatedIntegrationEvent? AddedEvent { get; private set; }
    public SupportActivity? UpdatedActivity { get; private set; }
    public SupportActivityUpdatedIntegrationEvent? UpdatedEvent { get; private set; }
    public SupportActivity? DeletedActivity { get; private set; }
    public SupportActivityDeletedIntegrationEvent? DeletedEvent { get; private set; }
    public SupportActivity? RestoredActivity { get; private set; }
    public SupportActivityRestoredIntegrationEvent? RestoredEvent { get; private set; }

    public FakeSupportActivityDataStore(SupportActivity? existingActivity = null)
    {
        _existingActivity = existingActivity;
    }

    public Task<SupportActivity?> GetSupportActivityByIdAsync(Guid tenantId, Guid supportActivityId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingActivity);
    }

    public Task AddSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedActivity = supportActivity;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedActivity = supportActivity;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task SoftDeleteSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeletedActivity = supportActivity;
        DeletedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task RestoreSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        RestoredActivity = supportActivity;
        RestoredEvent = integrationEvent;
        return Task.CompletedTask;
    }
}