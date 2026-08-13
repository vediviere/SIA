using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;

public sealed class SupportScheduleUpdatedIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid SupportScheduleId { get; init; }
    public Guid SupportHourId { get; init; }
    public Guid ClassroomLabId { get; init; }
    public Guid AcademicPeriodId { get; init; }
    public string Day { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}
