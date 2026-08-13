using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;

public sealed class SupportScheduleRestoredIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid SupportScheduleId { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}
