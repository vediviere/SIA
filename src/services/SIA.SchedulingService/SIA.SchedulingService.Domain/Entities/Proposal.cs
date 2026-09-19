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
    ReviewVersion = 0;
    Status = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
  public Guid EducationalProgramId { get; private set; }
  public Guid AcademicPeriodId { get; private set; }
  public Guid DivisionHeadId { get; private set; }
  public ProposalStatus ProposalStatus { get; private set; }
  public int ReviewVersion { get; private set; }
  public bool Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }

  public void SubmitForReview()
  {
    if (!Status)
    {
      throw new InvalidOperationException("No se puede enviar a revisión una propuesta inactiva.");
    }

    if (ProposalStatus != ProposalStatus.Draft && ProposalStatus != ProposalStatus.RequiresCorrection)
    {
      throw new InvalidOperationException("Solo una propuesta en borrador o devuelta para corrección puede enviarse a revisión.");
    }

    ReviewVersion++;
    ProposalStatus = ProposalStatus.SubmittedForReview;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void Approve()
    {
        if (ProposalStatus != ProposalStatus.SubmittedForReview)
        {
            throw new InvalidOperationException("Solo una propuesta en revisión puede aprobarse.");
        }
        ProposalStatus = ProposalStatus.Approved;
        UpdatedAtUtc = DateTime.UtcNow;
    }

  public void RequireCorrection()
  {
    if (ProposalStatus != ProposalStatus.SubmittedForReview)
    {
      throw new InvalidOperationException("Solo una propuesta en revisión puede devolverse para corrección.");
    }

    ProposalStatus = ProposalStatus.RequiresCorrection;
    UpdatedAtUtc = DateTime.UtcNow;
  }
}

