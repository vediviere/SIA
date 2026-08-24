namespace SIA.SchedulingService.Contracts.Requests.SupportActivity;

public sealed record UpdateSupportActivityRequest
{
    public required string Activity { get; init; }
    public required string Observation { get; init; }
}