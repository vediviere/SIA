namespace SIA.IdentityService.Domain.Entities;

public sealed class Role
{
  private Role()
  {
  }

  public Role(string code, string description)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("El código del rol es obligatorio.", nameof(code));
    }

    if (string.IsNullOrWhiteSpace(description))
    {
      throw new ArgumentException("La descripción del rol es obligatoria.", nameof(description));
    }

    Id = Guid.NewGuid();
    Code = code.Trim();
    Description = description.Trim();
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public string Code { get; private set; } = string.Empty;

  public string Description { get; private set; } = string.Empty;

  public DateTime CreatedAtUtc { get; private set; }

  public DateTime? UpdatedAtUtc { get; private set; }
}
