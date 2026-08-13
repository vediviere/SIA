using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Infrastructure.Configurations;

internal class SupportScheduleConfiguration : IEntityTypeConfiguration<SupportSchedule>
{
    public void Configure(EntityTypeBuilder<SupportSchedule> builder)
    {
        builder.ToTable("SupportSchedules");

        builder.HasKey(supportSchedule => supportSchedule.Id);

        builder.Property(supportSchedule => supportSchedule.Id)
            .HasColumnName("SupportScheduleId")
            .ValueGeneratedNever();

        builder.Property(supportSchedule => supportSchedule.TenantId)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.SupportHourId)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.ClassroomLabId)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.AcademicPeriodId)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.Day)
            .HasMaxLength(20) 
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.StartTime)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.EndTime)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.Status)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.CreatedAtUtc)
            .IsRequired();

        builder.Property(supportSchedule => supportSchedule.UpdatedAtUtc);

        builder.HasOne(supportSchedule => supportSchedule.ClassroomLab)
            .WithMany()
            .HasForeignKey(supportSchedule => supportSchedule.ClassroomLabId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(supportSchedule => supportSchedule.ClassroomLabId);
        builder.HasIndex(supportSchedule => supportSchedule.SupportHourId);
        builder.HasIndex(supportSchedule => supportSchedule.AcademicPeriodId);

        builder.HasIndex(supportSchedule => new
        {
            supportSchedule.TenantId,
            supportSchedule.ClassroomLabId,
            supportSchedule.Day,
            supportSchedule.StartTime
        });

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_SupportSchedules_TimeRange_Valid",
                "[StartTime] < [EndTime]");
        });
    }
}
