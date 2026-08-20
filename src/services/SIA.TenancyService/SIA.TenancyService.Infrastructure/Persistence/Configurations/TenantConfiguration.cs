using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.TenancyService.Domain.Entities;

namespace SIA.TenancyService.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
  public void Configure(EntityTypeBuilder<Tenant> builder)
  {
    builder.ToTable("Tenants");

    builder.HasKey(tenant => tenant.Id);

    builder.Property(tenant => tenant.Id)
      .HasColumnName("TenantId")
      .ValueGeneratedNever();

    builder.Property(tenant => tenant.InstituteCode)
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(tenant => tenant.Name)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(tenant => tenant.EmailDomain)
      .HasMaxLength(253)
      .IsRequired();

    builder.Property(tenant => tenant.IsActive)
      .IsRequired();

    builder.Property(tenant => tenant.CreatedAtUtc)
      .IsRequired();

    builder.Property(tenant => tenant.UpdatedAtUtc);

    builder.HasIndex(tenant => tenant.InstituteCode)
      .IsUnique();
  }
}
