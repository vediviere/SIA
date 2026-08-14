using Microsoft.EntityFrameworkCore;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Infrastructure.Persistence.Entities;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.IdentityService.Infrastructure.Persistence.Contexts;

public sealed class IdentityDbContext : DbContext
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Permission> Permissions => Set<Permission>();
  public DbSet<UserRole> UserRoles => Set<UserRole>();
  public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
  }
}
