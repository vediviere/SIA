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

    public bool ScheduleAdded { get; private set; }
    public bool ScheduleUpdated { get; private set; }
    public bool ScheduleDeleted { get; private set; }
    public bool ScheduleRestored { get; private set; }

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
        ScheduleAdded = true;
        return Task.CompletedTask;
    }

    public Task UpdateSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleUpdated = true;
        return Task.CompletedTask;
    }

    public Task SoftDeleteSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleDeleted = true;
        return Task.CompletedTask;
    }

    public Task RestoreSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleRestored = true;
        return Task.CompletedTask;
    }
}