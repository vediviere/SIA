
namespace SIA.AcademicService.Application.DTOs.AcademicPeriod;

public sealed class AcademicPeriodDto
{
    public Guid Id { get; init; }

    public Guid TenantId { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public DateOnly AcademicLoadProcessStartDate { get; init; }

    public DateOnly AcademicLoadProcessEndDate { get; init; }

    public DateOnly EnrollmentProcessStartDate { get; init; }

    public DateOnly EnrollmentProcessEndDate { get; init; }

    public DateOnly PlanningSubmissionDate { get; init; }

    public DateOnly FirstPartialGradeReportDate { get; init; }

    public DateOnly SecondPartialGradeReportDate { get; init; }

    public DateOnly ThirdPartialGradeReportDate { get; init; }

    public DateOnly FinalMinutesSubmissionDate { get; init; }

    public bool Status { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}