namespace SIA.SchedulingService.Domain.Entities;

public sealed class AcademicLoad
{
  private AcademicLoad()
  {
  }
  public AcademicLoad(Guid tenantId, Guid proposalId, Guid teacherId, Guid divisionId, Guid academicPeriodId, string officialLetterNumber, DateTime proposedDate, int classHours, int supportHours,
      DateTime assignmentDate)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenantId es obligatorio.", nameof(tenantId));
    }

    if (proposalId == Guid.Empty)
    {
      throw new ArgumentException("La propuesta de carga académica es obligatoria.", nameof(proposalId));
    }

    if (teacherId == Guid.Empty)
    {
      throw new ArgumentException("El docente es obligatorio.", nameof(teacherId));
    }

    if (divisionId == Guid.Empty)
    {
      throw new ArgumentException("La división es obligatoria.", nameof(divisionId));
    }

    if (academicPeriodId == Guid.Empty)
    {
      throw new ArgumentException("El periodo académico es obligatorio.", nameof(academicPeriodId));
    }

    if (string.IsNullOrWhiteSpace(officialLetterNumber))
    {
      throw new ArgumentException("El número de oficio es obligatorio.", nameof(officialLetterNumber));
    }

    if (classHours < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(classHours), "Las horas clase no pueden ser negativas, Tampoco pueden ser 0");
    }

    if (supportHours < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(supportHours), "Las horas de apoyo no pueden ser negativas, Tampoco pueden ser 0");
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    ProposalId = proposalId;
    TeacherId = teacherId;
    DivisionId = divisionId;
    AcademicPeriodId = academicPeriodId;
    OfficialLetterNumber = officialLetterNumber.Trim();
    ProposedDate = proposedDate;
    ClassHours = classHours;
    SupportHours = supportHours;
    AssignmentDate = assignmentDate;
    Status = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
  public Guid ProposalId { get; private set; }
  public Guid TeacherId { get; private set; }
  public Guid DivisionId { get; private set; }
  public Guid AcademicPeriodId { get; private set; }
  public string OfficialLetterNumber { get; private set; } = string.Empty;
  public DateTime ProposedDate { get; private set; }
  public int ClassHours { get; private set; }
  public int SupportHours { get; private set; }
  public DateTime AssignmentDate { get; private set; }
  public bool Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }

  public void Update(string officialLetterNumber, DateTime proposedDate, DateTime assignmentDate)
  {

    if (string.IsNullOrWhiteSpace(officialLetterNumber))
    {
      throw new ArgumentException("El número de oficio es obligatorio.", nameof(officialLetterNumber));
    }

    OfficialLetterNumber = officialLetterNumber.Trim();
    ProposedDate = proposedDate;
    AssignmentDate = assignmentDate;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void SetClassHours(int classHours)
  {
    if (classHours < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(classHours), "Class hours cannot be negative.");
    }

    ClassHours = classHours;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void SetSupportHours(int supportHours)
  {
    if (supportHours < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(supportHours), "Support hours cannot be negative.");
    }

    SupportHours = supportHours;
    UpdatedAtUtc = DateTime.UtcNow;
  }

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

}
