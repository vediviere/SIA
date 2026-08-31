
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class AcademicOfferingConfiguration : IEntityTypeConfiguration<AcademicOffering>
{
  public void Configure(EntityTypeBuilder<AcademicOffering> builder)
  {
    builder.ToTable("AcademicOffering");

    builder.HasKey(offering => offering.Id);
    builder.Property(offering => offering.Id).HasColumnName("OfferingId").ValueGeneratedNever();

    builder.Property(offering => offering.TenantId).IsRequired();
    builder.Property(offering => offering.GroupId).IsRequired();
    builder.Property(offering => offering.SubjectId).IsRequired();
    builder.Property(offering => offering.AcademicLoadId).IsRequired();
    builder.Property(offering => offering.OfferingStatus).HasMaxLength(20).IsRequired();
    builder.Property(offering => offering.ClassHours).HasDefaultValue(0).IsRequired();
    builder.Property(offering => offering.Status).IsRequired();
    builder.Property(offering => offering.CreatedAtUtc).IsRequired();
    builder.Property(offering => offering.UpdatedAtUtc);
    builder.HasIndex(offering => new { offering.TenantId, offering.GroupId, offering.SubjectId }).IsUnique();

    builder.HasOne<Group>().WithMany().HasForeignKey(offering => offering.GroupId).OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<AcademicLoad>().WithMany().HasForeignKey(offering => offering.AcademicLoadId).OnDelete(DeleteBehavior.Restrict);
  }
}
