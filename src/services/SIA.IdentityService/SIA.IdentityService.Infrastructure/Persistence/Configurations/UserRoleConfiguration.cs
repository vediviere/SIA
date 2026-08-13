using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
  public void Configure(EntityTypeBuilder<UserRole> builder)
  {
    builder.ToTable("UserRoles");

    builder.HasKey(userRole => userRole.Id);

    builder.Property(userRole => userRole.Id)
        .HasColumnName("UserRoleId")
        .ValueGeneratedNever();

    builder.Property(userRole => userRole.UserId)
        .IsRequired();

    builder.Property(userRole => userRole.RoleId)
        .IsRequired();

    builder.Property(userRole => userRole.CreatedAtUtc)
        .IsRequired();

    builder.Property(userRole => userRole.RevokedAtUtc);

    builder.HasOne<User>()
        .WithMany()
        .HasForeignKey(userRole => userRole.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Role>()
        .WithMany()
        .HasForeignKey(userRole => userRole.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(userRole => new
    {
      userRole.UserId,
      userRole.RoleId
    })
    .IsUnique()
    .HasFilter("[RevokedAtUtc] IS NULL");
  }
}
