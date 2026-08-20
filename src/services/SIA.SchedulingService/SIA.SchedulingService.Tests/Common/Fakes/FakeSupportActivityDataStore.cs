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

    public bool ActivityAdded { get; private set; }
    public bool ActivityUpdated { get; private set; }
    public bool ActivityDeleted { get; private set; }
    public bool ActivityRestored { get; private set; }

    public SupportActivity? AddedActivity { get; private set; }
    public SupportActivityCreatedIntegrationEvent? AddedEvent { get; private set; }
    public SupportActivityUpdatedIntegrationEvent? UpdatedEvent { get; private set; }
    public SupportActivityDeletedIntegrationEvent? DeletedEvent { get; private set; }
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
        ActivityAdded = true;
        AddedActivity = supportActivity;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivityUpdated = true;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task SoftDeleteSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivityDeleted = true;
        DeletedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task RestoreSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivityRestored = true;
        RestoredEvent = integrationEvent;
        return Task.CompletedTask;
    }
}