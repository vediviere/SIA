using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Domain.Entities;

public sealed class User
{
  private User()
  {
  }

  public User(Guid tenantId, string email, string passwordHash, bool mustChangePassword = false)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("La institución es obligatoria.", nameof(tenantId));
    }

    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(email));
    }

    if (string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new ArgumentException("La contraseña protegida es obligatoria.", nameof(passwordHash));
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    Email = email.Trim().ToLowerInvariant();
    PasswordHash = passwordHash;
    Status = UserStatus.Active;
    MustChangePassword = mustChangePassword;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid TenantId { get; private set; }

  public string Email { get; private set; } = string.Empty;

  public string PasswordHash { get; private set; } = string.Empty;

  public UserStatus Status { get; private set; }

  public bool MustChangePassword { get; private set; }

  public DateTime CreatedAtUtc { get; private set; }

  public DateTime? UpdatedAtUtc { get; private set; }

  public void Lock()
  {
    Status = UserStatus.Locked;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    Status = UserStatus.Inactive;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void Activate()
  {
    Status = UserStatus.Active;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void ChangePassword(string passwordHash)
  {
    if (string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new ArgumentException("La contraseña protegida es obligatoria.", nameof(passwordHash));
    }

    PasswordHash = passwordHash;
    MustChangePassword = false;
    UpdatedAtUtc = DateTime.UtcNow;
  }
}
