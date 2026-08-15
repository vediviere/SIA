namespace SIA.AcademicStaffService.Contracts.Requests.Professors;

public sealed record CreateTeacherRequest
{
    public required Guid TenantId { get; init; }

    public required Guid PersonId { get; init; }

    public required string ProfessionalProfile { get; init; }

    public required string ContractType { get; init; }

    public required int ContractHours { get; init; }
}