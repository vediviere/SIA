namespace SIA.AcademicStaffService.Contracts.Requests.Professors;

public sealed record CreateProfessorRequest
{
    public required Guid TenantId { get; init; }

    public required Guid PersonId { get; init; }

    public required string AcademicDegree { get; init; }

    public required string ProfessionalProfile { get; init; }

    public required string ContractType { get; init; }

    public required int ContractHours { get; init; }
}