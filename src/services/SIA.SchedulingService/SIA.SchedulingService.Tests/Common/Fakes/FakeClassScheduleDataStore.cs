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

    public ClassSchedule? AddedSchedule { get; private set; }
    public ClassScheduleCreatedIntegrationEvent? AddedEvent { get; private set; }
    public ClassSchedule? UpdatedSchedule { get; private set; }
    public ClassScheduleUpdatedIntegrationEvent? UpdatedEvent { get; private set; }
    public ClassSchedule? DeletedSchedule { get; private set; }
    public ClassScheduleDeletedIntegrationEvent? DeletedEvent { get; private set; }
    public ClassSchedule? RestoredSchedule { get; private set; }
    public ClassScheduleRestoredIntegrationEvent? RestoredEvent { get; private set; }

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
        AddedSchedule = classSchedule;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedSchedule = classSchedule;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task SoftDeleteClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeletedSchedule = classSchedule;
        DeletedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task RestoreClassScheduleWithOutboxAsync(ClassSchedule classSchedule, ClassScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        RestoredSchedule = classSchedule;
        RestoredEvent = integrationEvent;
        return Task.CompletedTask;
    }
}