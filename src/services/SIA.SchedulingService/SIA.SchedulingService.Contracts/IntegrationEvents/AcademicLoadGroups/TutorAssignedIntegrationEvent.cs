using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;

public sealed record TutorAssignedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid AcademicLoadGroupId { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required int Hours { get; init; }
    public int Version { get; init; } = 1;
}
