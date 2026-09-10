namespace SIA.SchedulingService.Contracts.Requests.SupportActivity;

public sealed record CreateSupportActivityRequest
{
    public required Guid TenantId { get; init; }
    public required string Activity { get; init; }
    public required string Observation { get; init; }
}