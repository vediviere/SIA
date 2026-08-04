namespace SIA.AcademicService.Contracts.Responses.AcademicPeriods;

public sealed record CreateAcademicPeriodResponse
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public required DateOnly StartDate { get; init; }

    public required DateOnly EndDate { get; init; }

    public required DateOnly AcademicLoadProcessStartDate { get; init; }

    public required DateOnly AcademicLoadProcessEndDate { get; init; }

    public required DateOnly EnrollmentProcessStartDate { get; init; }

    public required DateOnly EnrollmentProcessEndDate { get; init; }

    public required DateOnly PlanningSubmissionDate { get; init; }

    public required DateOnly FirstPartialGradeReportDate { get; init; }

    public required DateOnly SecondPartialGradeReportDate { get; init; }

    public required DateOnly ThirdPartialGradeReportDate { get; init; }

    public required DateOnly FinalMinutesSubmissionDate { get; init; }

    public required bool Status { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}