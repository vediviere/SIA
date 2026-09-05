namespace SIA.AdminBff.Clients.Academic;

public sealed record AcademicContextDto
{
  public required AcademicPeriodDto AcademicPeriod { get; init; }
  public required EducationalProgramDto EducationalProgram { get; init; }
  public required StudyPlanDto StudyPlan { get; init; }
  public required IReadOnlyCollection<SubjectDto> Subjects { get; init; }
  public required bool IsWithinPlanningWindow { get; init; }
}

public sealed record AcademicPeriodDto
{
  public required Guid Id { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required bool Status { get; init; }
  public required DateOnly AcademicLoadProcessStartDate { get; init; }
  public required DateOnly AcademicLoadProcessEndDate { get; init; }
}

public sealed record EducationalProgramDto
{
  public required Guid Id { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required string Level { get; init; }
}

public sealed record StudyPlanDto
{
  public required Guid Id { get; init; }
  public required Guid EducationalProgramId { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required string Version { get; init; }
  public required DateOnly EffectiveFrom { get; init; }
  public required bool Status { get; init; }
}

public sealed record SubjectDto
{
  public required Guid Id { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required int Semester { get; init; }
  public required int Credits { get; init; }
  public required bool IsRequired { get; init; }
}
