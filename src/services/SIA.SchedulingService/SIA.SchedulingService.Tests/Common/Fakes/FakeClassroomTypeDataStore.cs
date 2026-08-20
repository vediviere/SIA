using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeClassroomTypeDataStore : IClassroomTypeDataStore
{
    private readonly ClassroomType? _existingType;

    public bool ClassroomTypeAdded { get; private set; }
    public bool ClassroomTypeUpdated { get; private set; }
    public bool ClassroomTypeDeleted { get; private set; }
    public bool ClassroomTypeRestored { get; private set; }
    public bool NameExistsResult { get; set; }

    public FakeClassroomTypeDataStore(ClassroomType? existingType = null)
    {
        _existingType = existingType;
    }

    public Task<ClassroomType?> GetClassroomTypeByIdAsync(Guid tenantId, Guid classroomTypeId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingType);
    }

    public Task<bool> ClassroomTypeNameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(NameExistsResult);
    }

    public Task AddClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomTypeAdded = true;
        return Task.CompletedTask;
    }

    public Task UpdateClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomTypeUpdated = true;
        return Task.CompletedTask;
    }

    public Task SoftDeleteClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomTypeDeleted = true;
        return Task.CompletedTask;
    }

    public Task RestoreClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ClassroomTypeRestored = true;
        return Task.CompletedTask;
    }
}