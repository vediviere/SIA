namespace SIA.AcademicStaffService.Contracts.Responses.Professors;

public sealed record UpdateTeacherResponse
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid PersonId { get; init; }

    public required string ProfessionalProfile { get; init; }

    public required string ContractType { get; init; }

    public required int ContractHours { get; init; }

    public required bool Status { get; init; }

    public required DateTime? UpdatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}