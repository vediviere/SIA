using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.SupportActivity;

public sealed class UpdateSupportActivityRequest
{
    public string Activity { get; init; } = string.Empty;
    public string Observation { get; init; } = string.Empty;
}