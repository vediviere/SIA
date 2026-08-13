using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
  public void Configure(EntityTypeBuilder<RolePermission> builder)
  {
    builder.ToTable("RolePermissions");

    builder.HasKey(rolePermission => rolePermission.Id);

    builder.Property(rolePermission => rolePermission.Id)
        .HasColumnName("RolePermissionId")
        .ValueGeneratedNever();

    builder.Property(rolePermission => rolePermission.RoleId)
        .IsRequired();

    builder.Property(rolePermission => rolePermission.PermissionId)
        .IsRequired();

    builder.Property(rolePermission => rolePermission.CreatedAtUtc)
        .IsRequired();

    builder.Property(rolePermission => rolePermission.RevokedAtUtc);

    builder.HasData(
  new
  {
    Id = Guid.Parse("103b2828-b32d-497a-87c0-64220bb4a79e"),
    RoleId = Guid.Parse("76fcb7de-5a4f-4dc4-b893-3e5799ad2c11"),
    PermissionId = Guid.Parse("37632285-0224-4997-b1e1-89cb50d13fa2"),
    CreatedAtUtc = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc),
    RevokedAtUtc = (DateTime?)null
  });

    builder.HasOne<Role>()
        .WithMany()
        .HasForeignKey(rolePermission => rolePermission.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Permission>()
        .WithMany()
        .HasForeignKey(rolePermission => rolePermission.PermissionId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(rolePermission => new
    {
      rolePermission.RoleId,
      rolePermission.PermissionId
    })
    .IsUnique()
    .HasFilter("[RevokedAtUtc] IS NULL");
  }
}
