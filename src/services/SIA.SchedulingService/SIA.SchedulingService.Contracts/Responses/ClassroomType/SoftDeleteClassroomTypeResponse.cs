using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Responses.ClassroomType;

public sealed class SoftDeleteClassroomTypeResponse
{
    public Guid Id { get; init; }
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}