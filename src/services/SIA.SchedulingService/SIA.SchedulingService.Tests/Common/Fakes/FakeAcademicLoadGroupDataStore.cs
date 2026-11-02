using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeAcademicLoadGroupDataStore : IAcademicLoadGroupDataStore
{
    private readonly AcademicLoadGroup? _assignmentReturn;
    private readonly bool _existsReturn;

    public FakeAcademicLoadGroupDataStore(AcademicLoadGroup? assignmentReturn = null, bool existsReturn = false)
    {
        _assignmentReturn = assignmentReturn;
        _existsReturn = existsReturn;
    }

    public AcademicLoadGroup? AddedAssignment { get; private set; }
    public AcademicLoadGroup? RemovedAssignment { get; private set; }
    public AcademicLoadGroup? ReplacedOldAssignment { get; private set; }
    public AcademicLoadGroup? ReplacedNewAssignment { get; private set; }
    public TutorAssignedIntegrationEvent? AddedAssignedEvent { get; private set; }
    public TutorRemovedIntegrationEvent? AddedRemovedEvent { get; private set; }

    public Task<bool> ExistsByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existsReturn);
    }

    public Task<AcademicLoadGroup?> GetByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        if (_assignmentReturn is null || _assignmentReturn.TenantId != tenantId || _assignmentReturn.GroupId != groupId)
        {
            return Task.FromResult<AcademicLoadGroup?>(null);
        }
        return Task.FromResult<AcademicLoadGroup?>(_assignmentReturn);
    }

    public Task AddAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup academicLoadGroup,
        TutorAssignedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        AddedAssignment = academicLoadGroup;
        AddedAssignedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task RemoveAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup academicLoadGroup,
        TutorRemovedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        RemovedAssignment = academicLoadGroup;
        AddedRemovedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ReplaceAcademicLoadGroupWithOutboxAsync(AcademicLoadGroup oldAssignment,
        AcademicLoadGroup newAssignment, TutorRemovedIntegrationEvent removedEvent,
        TutorAssignedIntegrationEvent assignedEvent,
        CancellationToken cancellationToken)
    {
        ReplacedOldAssignment = oldAssignment;
        ReplacedNewAssignment = newAssignment;
        AddedRemovedEvent = removedEvent;
        AddedAssignedEvent = assignedEvent;
        return Task.CompletedTask;
    }
}