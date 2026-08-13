using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");

    builder.HasKey(user => user.Id);

    builder.Property(user => user.Id)
        .HasColumnName("UserId")
        .ValueGeneratedNever();

    builder.Property(user => user.TenantId)
        .IsRequired();

    builder.Property(user => user.Email)
        .HasMaxLength(320)
        .IsRequired();

    builder.Property(user => user.PasswordHash)
        .HasMaxLength(500)
        .IsRequired();

    builder.Property(user => user.Status)
        .HasConversion<int>()
        .IsRequired();

    builder.Property(user => user.MustChangePassword)
        .IsRequired();

    builder.Property(user => user.CreatedAtUtc)
        .IsRequired();

    builder.Property(user => user.UpdatedAtUtc);

    builder.HasIndex(user => user.Email)
        .IsUnique();

    builder.HasIndex(user => user.TenantId);
  }
}
