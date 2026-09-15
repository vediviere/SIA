using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.IdentityService.Contracts.Enums;
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
    Code = RoleCode.Administrator.ToString(),
    Description = "Administrador institucional",
    CreatedAtUtc = new DateTime(2026, 8, 9, 0, 0, 0, DateTimeKind.Utc),
    UpdatedAtUtc = (DateTime?)null
  },
  new
  {
    Id = Guid.Parse("7e33d5d4-e87c-4ee1-9c72-61dd6267cc61"),
    Code = RoleCode.Teacher.ToString(),
    Description = "Docente",
    CreatedAtUtc = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc),
    UpdatedAtUtc = (DateTime?)null
  },
  new
  {
    Id = Guid.Parse("98161f96-b782-4283-b5fc-b55851df64d6"),
    Code = RoleCode.DivisionHead.ToString(),
    Description = "Jefe de división",
    CreatedAtUtc = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc),
    UpdatedAtUtc = (DateTime?)null
  },
  new
  {
    Id = Guid.Parse("81d305d5-3af0-4e58-a4e5-2c5b9f4dff18"),
    Code = RoleCode.Coordinator.ToString(),
    Description = "Coordinador",
    CreatedAtUtc = new DateTime(2026, 9, 15, 0, 0, 0, DateTimeKind.Utc),
    UpdatedAtUtc = (DateTime?)null
  },
  new
  {
    Id = Guid.Parse("9713d35e-3aee-42bf-920a-97dd1d00c16c"),
    Code = RoleCode.Student.ToString(),
    Description = "Estudiante",
    CreatedAtUtc = new DateTime(2026, 9, 15, 0, 0, 0, DateTimeKind.Utc),
    UpdatedAtUtc = (DateTime?)null
  });
  }
}
