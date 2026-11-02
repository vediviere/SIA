using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;

public sealed record ReplaceTutorRequest
{
    public required Guid GroupId { get; init; }
    public required Guid AcademicLoadId { get; init; }
}