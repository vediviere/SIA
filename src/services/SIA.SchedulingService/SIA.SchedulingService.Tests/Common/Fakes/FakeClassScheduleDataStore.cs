using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeClassScheduleDataStore : IClassScheduleDataStore
{
    private readonly ClassSchedule? _existingSchedule;

    public bool ScheduleAdded { get; private set; }
    public bool ScheduleUpdated { get; private set; }
    public bool ScheduleDeleted { get; private set; }
    public bool ScheduleRestored { get; private set; }

    public FakeClassScheduleDataStore(ClassSchedule? existingSchedule = null)
    {
        _existingSchedule = existingSchedule;
    }

    public Task<ClassSchedule?> GetClassScheduleByIdAsync(Guid tenantId, Guid classScheduleId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingSchedule);
    }

    public Task AddClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleAdded = true;
        return Task.CompletedTask;
    }

    public Task UpdateClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleUpdated = true;
        return Task.CompletedTask;
    }

    public Task SoftDeleteClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleDeleted = true;
        return Task.CompletedTask;
    }

    public Task RestoreClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ScheduleRestored = true;
        return Task.CompletedTask;
    }
}