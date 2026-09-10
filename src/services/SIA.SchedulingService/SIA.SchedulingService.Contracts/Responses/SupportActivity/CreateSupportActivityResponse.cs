namespace SIA.SchedulingService.Contracts.Responses.SupportActivity;

public sealed record CreateSupportActivityResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string Activity { get; init; }
    public required string Observation { get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}