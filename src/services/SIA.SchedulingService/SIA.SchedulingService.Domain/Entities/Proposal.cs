using SIA.SchedulingService.Domain.Enums;

namespace SIA.SchedulingService.Domain.Entities;

public sealed class Proposal
{
  private Proposal()
  {
  }

  public Proposal(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, Guid divisionHeadId)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenantId es obligatorio.", nameof(tenantId));
    }

    if (educationalProgramId == Guid.Empty)
    {
      throw new ArgumentException("El programa educativo es obligatorio.", nameof(educationalProgramId));
    }

    if (academicPeriodId == Guid.Empty)
    {
      throw new ArgumentException("El periodo académico es obligatorio.", nameof(academicPeriodId));
    }

    if (divisionHeadId == Guid.Empty)
    {
      throw new ArgumentException("El jefe de carrera es obligatorio.", nameof(divisionHeadId));
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    EducationalProgramId = educationalProgramId;
    AcademicPeriodId = academicPeriodId;
    DivisionHeadId = divisionHeadId;
    ProposalStatus = ProposalStatus.Draft;
    Status = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
  public Guid EducationalProgramId { get; private set; }
  public Guid AcademicPeriodId { get; private set; }
  public Guid DivisionHeadId { get; private set; }
  public ProposalStatus ProposalStatus { get; private set; }
  public bool Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }
}
