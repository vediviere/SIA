using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.DTOs.ClassroomTypes;

public sealed class ClassroomTypeFilter
{
    public Guid TenantId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
