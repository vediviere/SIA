using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Domain;

public sealed class RolePermissionTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateActiveRolePermission()
  {
    var roleId = Guid.NewGuid();
    var permissionId = Guid.NewGuid();

    var rolePermission = new RolePermission(roleId, permissionId);

    Assert.NotEqual(Guid.Empty, rolePermission.Id);
    Assert.Equal(roleId, rolePermission.RoleId);
    Assert.Equal(permissionId, rolePermission.PermissionId);
    Assert.NotEqual(default, rolePermission.CreatedAtUtc);
    Assert.Null(rolePermission.RevokedAtUtc);
    Assert.True(rolePermission.IsActive);
  }

  [Fact]
  public void Constructor_WithEmptyRoleId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new RolePermission(Guid.Empty, Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyPermissionId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new RolePermission(Guid.NewGuid(), Guid.Empty));
  }

  [Fact]
  public void Revoke_ShouldDeactivateRolePermission()
  {
    var rolePermission = new RolePermission(Guid.NewGuid(), Guid.NewGuid());

    rolePermission.Revoke();

    Assert.False(rolePermission.IsActive);
    Assert.NotNull(rolePermission.RevokedAtUtc);
  }

  [Fact]
  public void Revoke_WhenAlreadyRevoked_ShouldKeepOriginalRevokedDate()
  {
    var rolePermission = new RolePermission(Guid.NewGuid(), Guid.NewGuid());

    rolePermission.Revoke();

    var originalRevokedAtUtc = rolePermission.RevokedAtUtc;

    rolePermission.Revoke();

    Assert.Equal(originalRevokedAtUtc, rolePermission.RevokedAtUtc);
  }
}
