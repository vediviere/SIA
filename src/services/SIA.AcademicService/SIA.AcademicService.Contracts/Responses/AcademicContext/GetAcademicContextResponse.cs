using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Responses.AcademicContext;

public sealed record GetAcademicContextResponse
{
    public required AcademicPeriodContextDto AcademicPeriod { get; init; }
    public required EducationalProgramContextDto EducationalProgram { get; init; }
    public required StudyPlanContextDto StudyPlan { get; init; }
    public required IReadOnlyCollection<SubjectContextDto> Subjects { get; init; }
    public required bool IsWithinPlanningWindow { get; init; }
}

public sealed record AcademicPeriodContextDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required bool Status { get; init; }
    public required DateOnly AcademicLoadProcessStartDate { get; init; }
    public required DateOnly AcademicLoadProcessEndDate { get; init; }
}

public sealed record EducationalProgramContextDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Level { get; init; }
}

public sealed record StudyPlanContextDto
{
    public required Guid Id { get; init; }
    public required Guid EducationalProgramId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Version { get; init; }
    public required DateOnly EffectiveFrom { get; init; }
    public required bool Status { get; init; }
}

public sealed record SubjectContextDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int Semester { get; init; }
    public required int Credits { get; init; }
    public required bool IsRequired { get; init; }
}
