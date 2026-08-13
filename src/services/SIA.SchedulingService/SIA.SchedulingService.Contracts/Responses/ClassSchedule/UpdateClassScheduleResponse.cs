using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Responses.ClassSchedule;

public sealed class UpdateClassScheduleResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid OfferingId { get; init; }
    public Guid ClassroomLabId { get; init; }
    public Guid AcademicPeriodId { get; init; }
    public string Day { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}