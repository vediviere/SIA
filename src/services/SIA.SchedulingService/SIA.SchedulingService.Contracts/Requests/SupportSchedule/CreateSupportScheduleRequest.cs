using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.SupportSchedules;

public sealed class CreateSupportScheduleRequest
{
    public Guid TenantId { get; init; }
    public Guid SupportHourId { get; init; }
    public Guid ClassroomLabId { get; init; }
    public Guid AcademicPeriodId { get; init; }
    public string Day { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}