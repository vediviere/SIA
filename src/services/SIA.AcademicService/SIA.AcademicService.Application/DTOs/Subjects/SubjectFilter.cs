namespace SIA.AcademicService.Application.DTOs.Subjects;

public sealed record SubjectFilter
{

    public string? Code { get; init; }

    public string? Name { get; init; }

    public int? Semester { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}