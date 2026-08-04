namespace SIA.AcademicService.Contracts.Responses.EducationalProgramsResponse;

public sealed class UpdateEducationalProgramsResponse
{
    public required Guid Id { get; set; }

    public required Guid TenantId { get; set; }

    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string Level { get; set; }

    public required bool Status { get; set; }

    public required DateTime UpdatedAtUtc { get; set; }
}
