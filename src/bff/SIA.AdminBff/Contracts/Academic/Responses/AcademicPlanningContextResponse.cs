using SIA.AdminBff.Contracts.AcademicStaff.Responses;

namespace SIA.AdminBff.Contracts.Academic.Responses;

public sealed record AcademicPlanningContextResponse
{
  public required AcademicPeriodResponse AcademicPeriod { get; init; }
  public required EducationalProgramResponse EducationalProgram { get; init; }
  public required StudyPlanResponse StudyPlan { get; init; }
  public required IReadOnlyCollection<SubjectResponse> Subjects { get; init; }
  public required IReadOnlyCollection<TeacherCandidateResponse> TeacherCandidates { get; init; }
  public required bool IsWithinPlanningWindow { get; init; }
}

public sealed record AcademicPeriodResponse
{
  public required Guid Id { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required bool IsActive { get; init; }
  public required DateOnly PlanningStartDate { get; init; }
  public required DateOnly PlanningEndDate { get; init; }
}

public sealed record EducationalProgramResponse
{
  public required Guid Id { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required string Level { get; init; }
}

public sealed record StudyPlanResponse
{
  public required Guid Id { get; init; }
  public required Guid EducationalProgramId { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required string Version { get; init; }
  public required DateOnly EffectiveFrom { get; init; }
  public required bool IsActive { get; init; }
}

public sealed record SubjectResponse
{
  public required Guid Id { get; init; }
  public required string Code { get; init; }
  public required string Name { get; init; }
  public required int Semester { get; init; }
  public required int Credits { get; init; }
  public required bool IsRequired { get; init; }
}
