namespace SIA.SchedulingService.Contracts.Responses.Building;

public sealed record CreateBuildingResponse
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required bool Status { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}