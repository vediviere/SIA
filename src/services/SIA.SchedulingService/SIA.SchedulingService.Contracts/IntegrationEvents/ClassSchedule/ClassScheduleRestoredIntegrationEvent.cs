using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;

public sealed class ClassScheduleRestoredIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid ClassScheduleId { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}