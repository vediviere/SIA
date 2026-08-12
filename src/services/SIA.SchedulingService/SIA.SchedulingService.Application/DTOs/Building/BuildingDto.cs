namespace SIA.SchedulingService.Application.DTOs.Building;

public sealed record BuildingDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}