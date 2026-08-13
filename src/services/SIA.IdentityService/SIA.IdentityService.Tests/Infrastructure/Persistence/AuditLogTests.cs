using SIA.IdentityService.Infrastructure.Persistence.Entities;

namespace SIA.IdentityService.Tests.Infrastructure.Persistence;

public sealed class AuditLogTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateAuditLog()
  {
    var tenantId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var auditLog = new AuditLog(tenantId, "RoleAssigned", "UserRole", Guid.NewGuid().ToString(),  correlationId, userId);

    Assert.NotEqual(Guid.Empty, auditLog.Id);
    Assert.Equal(tenantId, auditLog.TenantId);
    Assert.Equal("RoleAssigned", auditLog.Action);
    Assert.Equal("UserRole", auditLog.EntityName);
    Assert.Equal(userId, auditLog.UserId);
    Assert.Equal(correlationId, auditLog.CorrelationId);
    Assert.NotEqual(default, auditLog.OccurredAtUtc);
  }

  [Fact]
  public void Constructor_WithoutUserId_ShouldAllowSystemOperation()
  {
    var auditLog = new AuditLog(Guid.NewGuid(), "StudentRoleAssigned", "UserRole", Guid.NewGuid().ToString(), Guid.NewGuid());

    Assert.Null(auditLog.UserId);
  }

  [Fact]
  public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new AuditLog(Guid.Empty, "RoleAssigned", "UserRole", Guid.NewGuid().ToString(), Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyAction_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new AuditLog(Guid.NewGuid(), "", "UserRole", Guid.NewGuid().ToString(), Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyCorrelationId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new AuditLog(Guid.NewGuid(), "RoleAssigned", "UserRole", Guid.NewGuid().ToString(),    Guid.Empty));
  }
}
