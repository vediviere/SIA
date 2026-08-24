namespace SIA.SchedulingService.Application.DTOs.Classrooms;

public sealed record ClassroomLabFilter
{
    public required Guid TenantId { get; init; }
    public Guid? BuildingId { get; init; }
    public Guid? ClassroomTypeId { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public bool? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}