namespace SIA.AcademicService.Contracts.Requests.Subjects;

public sealed record CreateSubjectRequest
{
  public required Guid TenantId { get; init; }

  public required string Code { get; init; }

  public required string Name { get; init; }

  public required int Semester { get; init; }

  public required int TheoryHours { get; init; }

  public required int PracticeHours { get; init; }

  public required int Credits { get; init; }
}
