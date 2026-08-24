using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeSupportScheduleDataStore : ISupportScheduleDataStore
{
    private readonly SupportSchedule? _existingSchedule;

    public SupportSchedule? AddedSchedule { get; private set; }
    public SupportScheduleCreatedIntegrationEvent? AddedEvent { get; private set; }
    public SupportSchedule? UpdatedSchedule { get; private set; }
    public SupportScheduleUpdatedIntegrationEvent? UpdatedEvent { get; private set; }
    public SupportSchedule? DeletedSchedule { get; private set; }
    public SupportScheduleDeletedIntegrationEvent? DeletedEvent { get; private set; }
    public SupportSchedule? RestoredSchedule { get; private set; }
    public SupportScheduleRestoredIntegrationEvent? RestoredEvent { get; private set; }

    public FakeSupportScheduleDataStore(SupportSchedule? existingSchedule = null)
    {
        _existingSchedule = existingSchedule;
    }

    public Task<SupportSchedule?> GetSupportScheduleByIdAsync(Guid tenantId, Guid supportScheduleId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingSchedule);
    }

    public Task AddSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedSchedule = supportSchedule;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedSchedule = supportSchedule;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task SoftDeleteSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeletedSchedule = supportSchedule;
        DeletedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task RestoreSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        RestoredSchedule = supportSchedule;
        RestoredEvent = integrationEvent;
        return Task.CompletedTask;
    }
}