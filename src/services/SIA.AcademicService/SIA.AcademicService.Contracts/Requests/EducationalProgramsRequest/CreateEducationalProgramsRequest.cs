namespace SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
public sealed record CreateEducationalProgramsRequest
{

    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string Level { get; set; }

}
