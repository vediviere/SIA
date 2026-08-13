using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;

public sealed class ClassroomLabDeletedIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid ClassroomLabId { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}
