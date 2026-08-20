namespace SIA.TenancyService.Domain.Entities;

public sealed class Tenant
{
  private Tenant()
  {
  }

  public Tenant(string instituteCode, string name, string emailDomain)
  {
    if (string.IsNullOrWhiteSpace(instituteCode))
    {
      throw new ArgumentException("El código institucional es obligatorio.", nameof(instituteCode));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("El nombre de la institución es obligatorio.", nameof(name));
    }

    if (string.IsNullOrWhiteSpace(emailDomain))
    {
      throw new ArgumentException("El dominio de correo es obligatorio.", nameof(emailDomain));
    }

    Id = Guid.NewGuid();
    InstituteCode = instituteCode.Trim().ToUpperInvariant();
    Name = name.Trim();
    EmailDomain = emailDomain.Trim().TrimStart('@').ToLowerInvariant();
    IsActive = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public string InstituteCode { get; private set; } = string.Empty;
  public string Name { get; private set; } = string.Empty;
  public string EmailDomain { get; private set; } = string.Empty;
  public bool IsActive { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }

  public bool AllowsEmail(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      return false;
    }

    var normalizedEmail = email.Trim().ToLowerInvariant();
    var separatorIndex = normalizedEmail.LastIndexOf('@');

    if (separatorIndex <= 0 || separatorIndex == normalizedEmail.Length - 1)
    {
      return false;
    }

    var domain = normalizedEmail[(separatorIndex + 1)..];

    return string.Equals(domain, EmailDomain, StringComparison.OrdinalIgnoreCase);
  }

  public void Activate()
  {
    IsActive = true;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    IsActive = false;
    UpdatedAtUtc = DateTime.UtcNow;
  }
}
