using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Entities.Academics;

namespace SIA.SchedulingService.Infrastructure.Persistence.Configurations;

internal sealed class AcademicLoadGroupConfiguration : IEntityTypeConfiguration<AcademicLoadGroup>
{
    public void Configure(EntityTypeBuilder<AcademicLoadGroup> builder)
    {
        builder.ToTable("AcademicLoadGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasColumnName("AcademicLoadGroupId")
               .ValueGeneratedNever();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.AcademicLoadId).IsRequired();
        builder.Property(x => x.GroupId).IsRequired();
        builder.Property(x => x.Hours).IsRequired();

        builder.HasOne<AcademicLoad>()
               .WithMany()
               .HasForeignKey(x => x.AcademicLoadId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Group>()
               .WithMany()
               .HasForeignKey(x => x.GroupId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AcademicLoadId);

        builder.HasIndex(x => new { x.TenantId, x.GroupId }).IsUnique();
    }
}