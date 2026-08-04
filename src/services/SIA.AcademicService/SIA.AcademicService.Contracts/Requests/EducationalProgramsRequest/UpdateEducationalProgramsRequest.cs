namespace SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;

public sealed class UpdateEducationalProgramsRequest
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string Level { get; set; }
}
