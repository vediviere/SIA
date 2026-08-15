namespace SIA.SchedulingService.Application.DTOs.AcademicOffering;

public sealed class AcademicOfferingDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid GroupId { get; init; }
    public Guid SubjectId { get; init; }
    public Guid AcademicLoadId { get; init; }
    public string OfferingStatus { get; init; }
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}