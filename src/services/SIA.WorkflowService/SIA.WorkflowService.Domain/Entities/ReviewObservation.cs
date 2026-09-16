using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Domain.Entities;

public sealed class ReviewObservation
{
  private ReviewObservation()
  {
  }

  public ReviewObservation(Guid reviewProcessId, Guid tenantId, ObservationTarget targetType, Guid targetId, string description, Guid createdBy, DateTime createdAtUtc, Guid correlationId)
  {
    if (reviewProcessId == Guid.Empty)
    {
      throw new ArgumentException("El proceso de revisión es obligatorio.", nameof(reviewProcessId));
    }

    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
    }

    if (!Enum.IsDefined(targetType))
    {
      throw new ArgumentException("El tipo de elemento observado no es válido.", nameof(targetType));
    }

    if (targetId == Guid.Empty)
    {
      throw new ArgumentException("El elemento observado es obligatorio.", nameof(targetId));
    }

    if (string.IsNullOrWhiteSpace(description))
    {
      throw new ArgumentException("La descripción de la observación es obligatoria.", nameof(description));
    }

    if (createdBy == Guid.Empty)
    {
      throw new ArgumentException("El usuario que registró la observación es obligatorio.", nameof(createdBy));
    }

    if (createdAtUtc == default)
    {
      throw new ArgumentException("La fecha de la observación es obligatoria.", nameof(createdAtUtc));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    Id = Guid.NewGuid();
    ReviewProcessId = reviewProcessId;
    TenantId = tenantId;
    TargetType = targetType;
    TargetId = targetId;
    Description = description.Trim();
    CreatedBy = createdBy;
    CreatedAtUtc = createdAtUtc;
    CorrelationId = correlationId;
  }

  public Guid Id { get; private set; }
  public Guid ReviewProcessId { get; private set; }
  public Guid TenantId { get; private set; }
  public ObservationTarget TargetType { get; private set; }
  public Guid TargetId { get; private set; }
  public string Description { get; private set; } = string.Empty;
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public Guid CorrelationId { get; private set; }
}
