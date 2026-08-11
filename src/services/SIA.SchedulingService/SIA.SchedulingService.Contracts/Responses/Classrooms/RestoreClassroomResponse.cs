using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Responses.Classrooms;

public sealed class RestoreClassroomResponse
{
    public Guid Id { get; init; }
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}