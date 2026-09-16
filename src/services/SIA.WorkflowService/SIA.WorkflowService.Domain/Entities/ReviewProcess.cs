using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Domain.Entities;

public sealed class ReviewProcess
{
  private readonly List<ReviewObservation> _observations = [];

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
  public Guid? DecidedBy { get; private set; }
  public DateTime? DecidedAtUtc { get; private set; }
  public Guid? DecisionCorrelationId { get; private set; }
  public IReadOnlyCollection<ReviewObservation> Observations => _observations;

  public void Approve(Guid decidedBy, DateTime decidedAtUtc, Guid correlationId)
  {
    ValidateDecision(decidedBy, decidedAtUtc, correlationId);
    Complete(ReviewStatus.Approved, decidedBy, decidedAtUtc, correlationId);
  }

  public void ReturnForCorrection(Guid decidedBy, DateTime decidedAtUtc, Guid correlationId, IReadOnlyCollection<ReviewObservation> observations)
  {
    ValidateDecision(decidedBy, decidedAtUtc, correlationId);

    if (observations is null || observations.Count == 0)
    {
      throw new ArgumentException("Debe existir al menos una observación para regresar la propuesta.", nameof(observations));
    }

    foreach (var observation in observations)
    {
      if (observation.ReviewProcessId != Id || observation.TenantId != TenantId)
      {
        throw new ArgumentException("La observación no pertenece al proceso de revisión.", nameof(observations));
      }

      if (observation.CreatedBy != decidedBy || observation.CorrelationId != correlationId)
      {
        throw new ArgumentException("La observación no corresponde a la decisión actual.", nameof(observations));
      }

      if (observation.TargetType == ObservationTarget.Proposal && observation.TargetId != AcademicLoadProposalId)
      {
        throw new ArgumentException("La observación general no corresponde a la propuesta revisada.", nameof(observations));
      }
    }

    _observations.AddRange(observations);
    Complete(ReviewStatus.RequiresCorrection, decidedBy, decidedAtUtc, correlationId);
  }

  private void ValidateDecision(Guid decidedBy, DateTime decidedAtUtc, Guid correlationId)
  {
    if (Status != ReviewStatus.InReview)
    {
      throw new InvalidOperationException("La revisión ya fue finalizada.");
    }

    if (decidedBy == Guid.Empty)
    {
      throw new ArgumentException("El usuario que toma la decisión es obligatorio.", nameof(decidedBy));
    }

    if (decidedAtUtc == default)
    {
      throw new ArgumentException("La fecha de decisión es obligatoria.", nameof(decidedAtUtc));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }
  }

  private void Complete(ReviewStatus status, Guid decidedBy, DateTime decidedAtUtc, Guid correlationId)
  {
    Status = status;
    DecidedBy = decidedBy;
    DecidedAtUtc = decidedAtUtc;
    DecisionCorrelationId = correlationId;
    UpdatedAtUtc = decidedAtUtc;
  }
}
