using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class SupportActivityConfiguration : IEntityTypeConfiguration<SupportActivity>
{
    public void Configure(EntityTypeBuilder<SupportActivity> builder)
    {
        builder.ToTable("SupportActivities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ActivityId")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Activity)
            .HasColumnType("varchar(255)") 
            .IsRequired();

        builder.Property(x => x.Observation)
            .HasColumnType("varchar(500)") 
            .IsRequired(); 

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}