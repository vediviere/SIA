namespace SIA.IdentityService.Domain.Entities;

public sealed class RolePermission
{
  private RolePermission()
  {
  }

  public RolePermission(Guid roleId, Guid permissionId)
  {
    if (roleId == Guid.Empty)
    {
      throw new ArgumentException("El rol es obligatorio.", nameof(roleId));
    }

    if (permissionId == Guid.Empty)
    {
      throw new ArgumentException("El permiso es obligatorio.", nameof(permissionId));
    }

    Id = Guid.NewGuid();
    RoleId = roleId;
    PermissionId = permissionId;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid RoleId { get; private set; }

  public Guid PermissionId { get; private set; }

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
