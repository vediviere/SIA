using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Persistence.Configurations;

public sealed class TeachingSupportHoursConfiguration : IEntityTypeConfiguration<TeachingSupportHour>
{
  public void Configure(EntityTypeBuilder<TeachingSupportHour> builder)
  {
    builder.ToTable("TeachingSupportHours");

    builder.HasKey(hours => hours.Id);

    builder.Property(hours => hours.Id).HasColumnName("SupportHourId").ValueGeneratedNever();

    builder.Property(hours => hours.TenantId).IsRequired();
    builder.Property(hours => hours.ActivityId).IsRequired();
    builder.Property(hours => hours.AcademicLoadId).IsRequired();
    builder.Property(hours => hours.Hours).IsRequired();
    builder.Property(hours => hours.Status).IsRequired();
    builder.Property(hours => hours.CreatedAtUtc).IsRequired();
    builder.Property(hours => hours.UpdatedAtUtc);
    builder.HasIndex(hours => new { hours.TenantId, hours.AcademicLoadId, hours.ActivityId }).IsUnique();

    builder.HasOne<SupportActivity>().WithMany().HasForeignKey(hours => hours.ActivityId).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne<AcademicLoad>().WithMany().HasForeignKey(hours => hours.AcademicLoadId).OnDelete(DeleteBehavior.Restrict);
  }
}
