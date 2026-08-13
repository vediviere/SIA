using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.ClassSchedule;

public sealed class UpdateClassScheduleRequest
{
    public string Day { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}