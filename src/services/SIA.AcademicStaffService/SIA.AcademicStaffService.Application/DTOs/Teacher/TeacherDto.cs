namespace SIA.AcademicStaffService.Application.DTOs.Teacher;

public sealed record TeacherDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid PersonId { get; init; }
    public string ProfessionalProfile { get; init; } = default!;
    public string ContractType { get; init; } = default!;
    public int ContractHours { get; init; }
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}