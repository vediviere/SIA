namespace SIA.SchedulingService.Contracts.Requests.Building;

public sealed record CreateBuildingRequest
{
    public required Guid TenantId { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }
}
