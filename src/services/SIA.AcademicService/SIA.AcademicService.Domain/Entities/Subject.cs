namespace SIA.AcademicService.Domain.Entities;

public sealed class Subject
{
  private Subject()
  {
  }

  public Subject(
      Guid tenantId,
      string code,
      string name,
      int credits)
  {
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
    TenantId = tenantId;
    Code = code.Trim().ToUpperInvariant();
    Name = name.Trim();
    Credits = credits;
    Status = "Active";
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid TenantId { get; private set; }

  public string Code { get; private set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;

  public int Credits { get; private set; }

  public string Status { get; private set; } = string.Empty;

  public DateTime CreatedAtUtc { get; private set; }

  public DateTime? UpdatedAtUtc { get; private set; }
}
