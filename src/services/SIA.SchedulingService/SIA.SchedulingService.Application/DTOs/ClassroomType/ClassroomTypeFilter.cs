using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.DTOs.ClassroomTypes;

public sealed record ClassroomTypeFilter
{
    public required Guid TenantId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Code { get; init; }
    public bool? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}