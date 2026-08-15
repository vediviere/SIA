using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;

public sealed class SupportActivityRestoredIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid SupportActivityId { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}