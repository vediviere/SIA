using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(building => building.Id);

        builder.Property(building => building.Id).HasColumnName("BuildingId").ValueGeneratedNever();

        builder.Property(building => building.TenantId).IsRequired();

        builder.Property(building => building.Code).HasMaxLength(100).IsRequired();

        builder.Property(building => building.Name).HasMaxLength(200).IsRequired();

        builder.Property(building => building.Description).HasMaxLength(1000).IsRequired();

        builder.Property(building => building.Status).IsRequired();

        builder.Property(building => building.CreatedAtUtc).IsRequired();

        builder.Property(building => building.UpdatedAtUtc);

        builder.HasIndex(building => new
        {
            building.TenantId,
            building.Code
        })
            .IsUnique();
    }
}