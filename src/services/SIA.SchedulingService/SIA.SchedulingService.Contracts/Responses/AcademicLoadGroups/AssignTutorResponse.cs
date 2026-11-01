using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Responses.AcademicLoadGroups;

public sealed record AssignTutorResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required Guid Hours { get; init; }
}
