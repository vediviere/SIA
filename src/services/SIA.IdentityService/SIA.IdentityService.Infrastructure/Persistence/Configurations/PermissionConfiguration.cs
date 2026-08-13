using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
  public void Configure(EntityTypeBuilder<Permission> builder)
  {
    builder.ToTable("Permissions");

    builder.HasKey(permission => permission.Id);

    builder.Property(permission => permission.Id)
        .HasColumnName("PermissionId")
        .ValueGeneratedNever();

    builder.Property(permission => permission.Code)
        .HasMaxLength(150)
        .IsRequired();

    builder.Property(permission => permission.Description)
        .HasMaxLength(300)
        .IsRequired();

    builder.HasData(
  new
  {
    Id = Guid.Parse("37632285-0224-4997-b1e1-89cb50d13fa2"),
    Code = "Users.Manage",
    Description = "Administrar usuarios y roles",
    CreatedAtUtc = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc),
    UpdatedAtUtc = (DateTime?)null
  });

    builder.Property(permission => permission.CreatedAtUtc)
        .IsRequired();

    builder.Property(permission => permission.UpdatedAtUtc);

    builder.HasIndex(permission => permission.Code)
        .IsUnique();
  }
}
