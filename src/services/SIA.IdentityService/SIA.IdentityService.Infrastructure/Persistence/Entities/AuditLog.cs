namespace SIA.IdentityService.Infrastructure.Persistence.Entities;

public sealed class AuditLog
{
  private AuditLog()
  {
  }

  public AuditLog(Guid tenantId, string action, string entityName, string entityId, Guid correlationId, Guid? userId = null, string? oldValues = null, string? newValues = null)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
    }

    if (string.IsNullOrWhiteSpace(action))
    {
      throw new ArgumentException("La acción auditada es obligatoria.", nameof(action));
    }

    if (string.IsNullOrWhiteSpace(entityName))
    {
      throw new ArgumentException("El nombre de la entidad auditada es obligatorio.", nameof(entityName));
    }

    if (string.IsNullOrWhiteSpace(entityId))
    {
      throw new ArgumentException("El identificador de la entidad auditada es obligatorio.", nameof(entityId));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    Action = action.Trim();
    EntityName = entityName.Trim();
    EntityId = entityId.Trim();
    UserId = userId;
    OccurredAtUtc = DateTime.UtcNow;
    OldValues = oldValues;
    NewValues = newValues;
    CorrelationId = correlationId;
  }

  public Guid Id { get; private set; }

  public Guid TenantId { get; private set; }

  public string Action { get; private set; } = string.Empty;

  public string EntityName { get; private set; } = string.Empty;

  public string EntityId { get; private set; } = string.Empty;

  public Guid? UserId { get; private set; }

  public DateTime OccurredAtUtc { get; private set; }

  public string? OldValues { get; private set; }

  public string? NewValues { get; private set; }

  public Guid CorrelationId { get; private set; }
}
