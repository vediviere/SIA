namespace SIA.SchoolControlService.Domain.Entities;

public sealed class SubjectReference
{
  private SubjectReference()
  {
  }

  public SubjectReference(Guid subjectId, Guid tenantId, string code, string name, int credits, string status)
  {
    if (subjectId == Guid.Empty)
    {
      throw new ArgumentException(
          "El identificador de la materia es obligatorio.",
          nameof(subjectId));
    }

    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException(
          "El tenant es obligatorio.",
          nameof(tenantId));
    }

    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException(
          "El código de la materia es obligatorio.",
          nameof(code));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException(
          "El nombre de la materia es obligatorio.",
          nameof(name));
    }

    if (credits <= 0)
    {
      throw new ArgumentOutOfRangeException(
          nameof(credits),
          "Los créditos deben ser mayores que cero.");
    }

    Id = Guid.NewGuid();
    SubjectId = subjectId;
    TenantId = tenantId;
    Code = code.Trim().ToUpperInvariant();
    Name = name.Trim();
    Credits = credits;
    Status = status.Trim();
    CreatedAtUtc = DateTime.UtcNow;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid SubjectId { get; private set; }

  public Guid TenantId { get; private set; }

  public string Code { get; private set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;

  public int Credits { get; private set; }

  public string Status { get; private set; } = string.Empty;

  public DateTime CreatedAtUtc { get; private set; }

  public DateTime UpdatedAtUtc { get; private set; }

  public void Update(
      string code,
      string name,
      int credits,
      string status)
  {
    Code = code.Trim().ToUpperInvariant();
    Name = name.Trim();
    Credits = credits;
    Status = status.Trim();
    UpdatedAtUtc = DateTime.UtcNow;
  }
}
