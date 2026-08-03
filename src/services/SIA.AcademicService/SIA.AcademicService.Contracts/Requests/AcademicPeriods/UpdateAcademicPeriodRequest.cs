namespace SIA.AcademicService.Contracts.Requests.AcademicPeriods;

public sealed class UpdateAcademicPeriodRequest
{
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
}
