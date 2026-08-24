using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Common.Fakes;

internal class FakeClassroomLabDataStore : IClassroomLabDataStore
{
    private readonly ClassroomLab? _existingLab;

    public ClassroomLab? AddedClassroomLab { get; private set; }
    public ClassroomLabCreatedIntegrationEvent? AddedEvent { get; private set; }
    public ClassroomLab? UpdatedClassroomLab { get; private set; }
    public ClassroomLabUpdatedIntegrationEvent? UpdatedEvent { get; private set; }
    public ClassroomLab? DeletedClassroomLab { get; private set; }
    public ClassroomLabDeletedIntegrationEvent? DeletedEvent { get; private set; }
    public ClassroomLab? RestoredClassroomLab { get; private set; }
    public ClassroomLabRestoredIntegrationEvent? RestoredEvent { get; private set; }

    public bool CodeExistsResult { get; set; }

    public FakeClassroomLabDataStore(ClassroomLab? existingLab = null)
    {
        _existingLab = existingLab;
    }

    public Task<ClassroomLab?> GetClassroomLabByIdAsync(Guid tenantId, Guid classroomLabId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingLab);
    }

    public Task<bool> ClassroomLabCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return Task.FromResult(CodeExistsResult);
    }

    public Task AddClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedClassroomLab = classroomLab;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedClassroomLab = classroomLab;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task SoftDeleteClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeletedClassroomLab = classroomLab;
        DeletedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task RestoreClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        RestoredClassroomLab = classroomLab;
        RestoredEvent = integrationEvent;
        return Task.CompletedTask;
    }
}