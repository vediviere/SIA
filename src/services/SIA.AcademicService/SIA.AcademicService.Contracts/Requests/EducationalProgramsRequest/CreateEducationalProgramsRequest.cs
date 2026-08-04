namespace SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
public sealed class CreateEducationalProgramsRequest
{
    public required Guid TenantId { get; set; }

    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string Level { get; set; }

}
