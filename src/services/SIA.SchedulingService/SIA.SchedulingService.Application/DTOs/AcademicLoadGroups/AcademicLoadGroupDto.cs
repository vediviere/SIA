using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.DTOs.AcademicLoadGroups;

public sealed record AcademicLoadGroupDto
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required Guid GroupId { get; init; }
    public required int Hours { get; init; }    
}
