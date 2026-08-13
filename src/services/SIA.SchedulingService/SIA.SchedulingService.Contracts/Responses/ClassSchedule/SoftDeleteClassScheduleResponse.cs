using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Responses.ClassSchedule;

public sealed class SoftDeleteClassScheduleResponse
{
    public Guid Id { get; init; }
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}
