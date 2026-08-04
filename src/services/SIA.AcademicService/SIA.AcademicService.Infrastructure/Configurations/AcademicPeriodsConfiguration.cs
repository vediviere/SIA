using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Configurations;

public sealed class AcademicPeriodsConfiguration : IEntityTypeConfiguration<AcademicPeriod>
{
    public void Configure(EntityTypeBuilder<AcademicPeriod> builder)
    {
        builder.ToTable("AcademicPeriods");

        builder.HasKey(AcademicPeriods => AcademicPeriods.Id);

        builder.Property(AcademicPeriods => AcademicPeriods.Id).HasColumnName("AcademicPeriodId").ValueGeneratedNever();

        builder.Property(AcademicPeriods => AcademicPeriods.TenantId).IsRequired();
        
        builder.Property(AcademicPeriods => AcademicPeriods.Code).HasMaxLength(30).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.Name).IsRequired().HasMaxLength(100);

        builder.Property(AcademicPeriods => AcademicPeriods.StartDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.EndDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.AcademicLoadProcessStartDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.AcademicLoadProcessEndDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.EnrollmentProcessStartDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.EnrollmentProcessEndDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.PlanningSubmissionDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.FirstPartialGradeReportDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.SecondPartialGradeReportDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.ThirdPartialGradeReportDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.FinalMinutesSubmissionDate).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.Status).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.CreatedAtUtc).IsRequired();

        builder.Property(AcademicPeriods => AcademicPeriods.UpdatedAtUtc);

        builder.HasIndex(academicPeriod => new
        {
            academicPeriod.TenantId,
            academicPeriod.Code
        })
            .IsUnique();


    }
}