namespace SIA.SchedulingService.Application.DTOs.Classrooms;

public sealed class ClassroomLabFilter
{
    public Guid TenantId { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? ClassroomTypeId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
