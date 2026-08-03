
namespace SIA.AcademicService.Domain.Entities;

public sealed class AcademicPeriod
{
    private AcademicPeriod() 
    { 
    }

    public AcademicPeriod(
        Guid tenantId,
        string code,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        DateOnly academicLoadProcessStartDate,
        DateOnly academicLoadProcessEndDate,
        DateOnly enrollmentProcessStartDate,
        DateOnly enrollmentProcessEndDate,
        DateOnly planningSubmissionDate,
        DateOnly firstPartialGradeReportDate,
        DateOnly secondPartialGradeReportDate,
        DateOnly thirdPartialGradeReportDate,
        DateOnly finalMinutesSubmissionDate
        )

    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.",nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("El código del periodo académico es obligatorio.",nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del periodo académico es obligatorio.",nameof(name));
        }

        if (endDate < startDate)
        {
            throw new ArgumentException( "La fecha de fin no puede ser anterior a la fecha de inicio.",nameof(endDate));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        AcademicLoadProcessStartDate = academicLoadProcessStartDate;
        AcademicLoadProcessEndDate = academicLoadProcessEndDate;
        EnrollmentProcessStartDate = enrollmentProcessStartDate;
        EnrollmentProcessEndDate = enrollmentProcessEndDate;
        PlanningSubmissionDate = planningSubmissionDate;
        FirstPartialGradeReportDate = firstPartialGradeReportDate;
        SecondPartialGradeReportDate = secondPartialGradeReportDate;
        ThirdPartialGradeReportDate = thirdPartialGradeReportDate;
        FinalMinutesSubmissionDate = finalMinutesSubmissionDate;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public DateOnly AcademicLoadProcessStartDate { get; private set; }
    public DateOnly AcademicLoadProcessEndDate { get; private set; }
    public DateOnly EnrollmentProcessStartDate { get; private set; }
    public DateOnly EnrollmentProcessEndDate { get; private set; }
    public DateOnly PlanningSubmissionDate { get; private set; }
    public DateOnly FirstPartialGradeReportDate { get; private set; }
    public DateOnly SecondPartialGradeReportDate { get; private set; }
    public DateOnly ThirdPartialGradeReportDate { get; private set; }
    public DateOnly FinalMinutesSubmissionDate { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Deactivate()
    {
        Status = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        string code,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        DateOnly academicLoadProcessStartDate,
        DateOnly academicLoadProcessEndDate,
        DateOnly enrollmentProcessStartDate,
        DateOnly enrollmentProcessEndDate,
        DateOnly planningSubmissionDate,
        DateOnly firstPartialGradeReportDate,
        DateOnly secondPartialGradeReportDate,
        DateOnly thirdPartialGradeReportDate,
        DateOnly finalMinutesSubmissionDate)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("El código del periodo académico es obligatorio.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del periodo académico es obligatorio.", nameof(name));
        }

        if (endDate < startDate)
        {
            throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.", nameof(endDate));
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        AcademicLoadProcessStartDate = academicLoadProcessStartDate;
        AcademicLoadProcessEndDate = academicLoadProcessEndDate;
        EnrollmentProcessStartDate = enrollmentProcessStartDate;
        EnrollmentProcessEndDate = enrollmentProcessEndDate;
        PlanningSubmissionDate = planningSubmissionDate;
        FirstPartialGradeReportDate = firstPartialGradeReportDate;
        SecondPartialGradeReportDate = secondPartialGradeReportDate;
        ThirdPartialGradeReportDate = thirdPartialGradeReportDate;
        FinalMinutesSubmissionDate = finalMinutesSubmissionDate;
        UpdatedAtUtc = DateTime.UtcNow;
    }


}