using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder.ToTable("Roles");

    builder.HasKey(role => role.Id);

    builder.Property(role => role.Id)
      .HasColumnName("RoleId")
      .ValueGeneratedNever();

    builder.Property(role => role.Code)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(role => role.Description)
      .HasMaxLength(300)
      .IsRequired();

    builder.Property(role => role.CreatedAtUtc)
      .IsRequired();

    builder.Property(role => role.UpdatedAtUtc);

    builder.HasIndex(role => role.Code)
      .IsUnique();

    builder.HasData(
      new
      {
        Id = Guid.Parse("76fcb7de-5a4f-4dc4-b893-3e5799ad2c11"),
        Code = "Administrator",
        Description = "Administrador institucional",
        CreatedAtUtc = new DateTime(2026, 8, 9, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAtUtc = (DateTime?)null
      },
      new
      {
        Id = Guid.Parse("7e33d5d4-e87c-4ee1-9c72-61dd6267cc61"),
        Code = "Teacher",
        Description = "Docente",
        CreatedAtUtc = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAtUtc = (DateTime?)null
      },
      new
      {
        Id = Guid.Parse("98161f96-b782-4283-b5fc-b55851df64d6"),
        Code = "CareerHead",
        Description = "Jefe de carrera",
        CreatedAtUtc = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAtUtc = (DateTime?)null
      });
  }
}
