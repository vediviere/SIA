using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.DTOs.SupportActivity;

public sealed record SupportActivityFilter
{
    public required Guid TenantId { get; init; }
    public string? Activity { get; init; }
    public bool? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}