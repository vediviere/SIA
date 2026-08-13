namespace SIA.IdentityService.Domain.Entities;

public sealed class UserRole
{
  private UserRole()
  {
  }

  public UserRole(Guid userId, Guid roleId)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
    }

    if (roleId == Guid.Empty)
    {
      throw new ArgumentException("El rol es obligatorio.", nameof(roleId));
    }

    Id = Guid.NewGuid();
    UserId = userId;
    RoleId = roleId;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid UserId { get; private set; }

  public Guid RoleId { get; private set; }

  public DateTime CreatedAtUtc { get; private set; }

  public DateTime? RevokedAtUtc { get; private set; }

  public bool IsActive => RevokedAtUtc is null;

  public void Revoke()
  {
    if (!IsActive)
    {
      return;
    }

    RevokedAtUtc = DateTime.UtcNow;
  }
}
