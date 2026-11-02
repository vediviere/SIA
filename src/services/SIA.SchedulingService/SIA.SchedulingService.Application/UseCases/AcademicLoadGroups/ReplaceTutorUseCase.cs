using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;

public sealed class ReplaceTutorUseCase
{
    private readonly IAcademicLoadGroupDataStore _dataStore;
    private readonly IAcademicLoadDataStore _academicLoadDataStore;

    public ReplaceTutorUseCase(
        IAcademicLoadGroupDataStore dataStore,
        IAcademicLoadDataStore academicLoadDataStore)
    {
        _dataStore = dataStore;
        _academicLoadDataStore = academicLoadDataStore;
    }

    public async Task<AssignTutorResponse> ExecuteAsync(Guid tenantId, ReplaceTutorRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var oldAssignment = await _dataStore.GetByGroupIdAsync(tenantId, request.GroupId, cancellationToken);
        if (oldAssignment is null)
        {
            throw new TutorAssignmentNotFoundException(request.GroupId);
        }
        var newAcademicLoad = await _academicLoadDataStore.GetByIdAsync(tenantId, request.AcademicLoadId, cancellationToken);
        if (newAcademicLoad is null || !newAcademicLoad.Status)
        {
            throw new AcademicLoadNotFoundException(request.AcademicLoadId);
        }
        var newAssignment = new AcademicLoadGroup(
            tenantId,
            request.AcademicLoadId,
            request.GroupId
        );
        var removedEvent = new TutorRemovedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = DateTime.UtcNow,
            TenantId = oldAssignment.TenantId,
            GroupId = oldAssignment.GroupId,
            AcademicLoadId = oldAssignment.AcademicLoadId,
            Version = 1
        };

        var assignedEvent = new TutorAssignedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = DateTime.UtcNow,
            TenantId = newAssignment.TenantId,
            AcademicLoadGroupId = newAssignment.Id,
            GroupId = newAssignment.GroupId,
            AcademicLoadId = newAssignment.AcademicLoadId,
            Hours = newAssignment.Hours,
            Version = 1
        };
        await _dataStore.ReplaceAcademicLoadGroupWithOutboxAsync(oldAssignment, newAssignment, removedEvent, assignedEvent, cancellationToken);
        return new AssignTutorResponse
        {
            Id = newAssignment.Id,
            TenantId = newAssignment.TenantId,
            GroupId = newAssignment.GroupId,
            AcademicLoadId = newAssignment.AcademicLoadId,
            Hours = newAssignment.Hours,
            CorrelationId = correlationId
        };
    }
}