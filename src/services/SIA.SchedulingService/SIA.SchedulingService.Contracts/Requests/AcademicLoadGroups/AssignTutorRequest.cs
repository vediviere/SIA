using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;

public sealed record AssignTutorRequest
{
    public required Guid GroupId { get; init; }
    public required Guid AcademicLoadAcademicLoadId { get; init; }
}
