using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Responses.SupportActivity;

public sealed class UpdateSupportActivityResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Activity { get; init; } = string.Empty;
    public string Observation { get; init; } = string.Empty;
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}