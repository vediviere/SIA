using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Domain.Entities;

public sealed class ReviewProcess
{
  private ReviewProcess()
  {
  }

  public ReviewProcess(Guid tenantId, Guid academicLoadProposalId, Guid divisionHeadId, int version, DateTime submittedAtUtc, Guid correlationId)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
    }

    if (academicLoadProposalId == Guid.Empty)
    {
      throw new ArgumentException("La propuesta de carga académica es obligatoria.", nameof(academicLoadProposalId));
    }

    if (divisionHeadId == Guid.Empty)
    {
      throw new ArgumentException("El jefe de división es obligatorio.", nameof(divisionHeadId));
    }

    if (version <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(version), "La versión debe ser mayor que cero.");
    }

    if (submittedAtUtc == default)
    {
      throw new ArgumentException("La fecha de envío es obligatoria.", nameof(submittedAtUtc));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    AcademicLoadProposalId = academicLoadProposalId;
    DivisionHeadId = divisionHeadId;
    Version = version;
    SubmittedAtUtc = submittedAtUtc;
    CorrelationId = correlationId;
    Status = ReviewStatus.InReview;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
  public Guid AcademicLoadProposalId { get; private set; }
  public Guid DivisionHeadId { get; private set; }
  public int Version { get; private set; }
  public ReviewStatus Status { get; private set; }
  public DateTime SubmittedAtUtc { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }
  public Guid CorrelationId { get; private set; }
}
