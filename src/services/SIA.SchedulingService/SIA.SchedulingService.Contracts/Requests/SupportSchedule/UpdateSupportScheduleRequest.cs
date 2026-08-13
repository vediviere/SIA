using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.SupportSchedules;

public sealed class UpdateSupportScheduleRequest
{
    public string Day { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}