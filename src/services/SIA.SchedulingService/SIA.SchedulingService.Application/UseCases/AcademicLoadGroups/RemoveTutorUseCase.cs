using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;

public sealed class RemoveTutorUseCase
{
    private readonly IAcademicLoadGroupDataStore _dataStore;

    public RemoveTutorUseCase(IAcademicLoadGroupDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, RemoveTutorRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var assignment = await _dataStore.GetByGroupIdAsync(tenantId, request.GroupId, cancellationToken);

        if (assignment is null)
        {
            throw new TutorAssignmentNotFoundException(request.GroupId);
        }
        var integrationEvent = new TutorRemovedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = DateTime.UtcNow,
            TenantId = assignment.TenantId,
            GroupId = assignment.GroupId,
            AcademicLoadId = assignment.AcademicLoadId,
            Version = 1
        };
        await _dataStore.RemoveAcademicLoadGroupWithOutboxAsync(assignment, integrationEvent, cancellationToken);
    }
}