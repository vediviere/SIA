using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("RefreshTokens");

    builder.HasKey(token => token.Id);

    builder.Property(token => token.Id).HasColumnName("RefreshTokenId").ValueGeneratedNever();
    builder.Property(token => token.UserId).IsRequired();
    builder.Property(token => token.TokenHash).HasMaxLength(64).IsRequired();
    builder.Property(token => token.CreatedAtUtc).IsRequired();
    builder.Property(token => token.ExpiresAtUtc).IsRequired();
    builder.Property(token => token.RevokedAtUtc);

    builder.HasIndex(token => token.TokenHash).IsUnique();
    builder.HasIndex(token => token.UserId);

    builder.HasOne<User>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Restrict);
  }
}
