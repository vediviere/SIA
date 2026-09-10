using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.DTOs.SupportSchedules;

public sealed record SupportScheduleFilter
{
    public required Guid TenantId { get; init; }
    public Guid? SupportHourId { get; init; }
    public Guid? ClassroomLabId { get; init; }
    public Guid? AcademicPeriodId { get; init; }
    public string? Day { get; init; }
    public bool? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}