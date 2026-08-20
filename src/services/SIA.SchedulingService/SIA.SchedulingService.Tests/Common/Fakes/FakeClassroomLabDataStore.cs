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

    public bool ClassroomLabAdded { get; private set; }
    public bool ClassroomLabUpdated { get; private set; }
    public bool ClassroomLabDeleted { get; private set; }
    public bool ClassroomLabRestored { get; private set; }
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
        ClassroomLabAdded = true;
        return Task.CompletedTask;
    }

    public Task UpdateClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomLabUpdated = true;
        return Task.CompletedTask;
    }

    public Task SoftDeleteClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomLabDeleted = true;
        return Task.CompletedTask;
    }

    public Task RestoreClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomLabRestored = true;
        return Task.CompletedTask;
    }
}