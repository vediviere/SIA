namespace SIA.AcademicStaffService.Contracts.Requests.Professors;

public sealed record UpdateTeacherRequest
{
    public required string AcademicDegree { get; init; }

    public required string ProfessionalProfile { get; init; }

    public required string ContractType { get; init; }

    public required int ContractHours { get; init; }
}