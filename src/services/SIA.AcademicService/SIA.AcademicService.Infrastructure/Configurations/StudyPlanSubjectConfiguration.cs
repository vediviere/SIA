using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Configurations;

public sealed class StudyPlanSubjectConfiguration
    : IEntityTypeConfiguration<StudyPlanSubject>
{
    public void Configure(
        EntityTypeBuilder<StudyPlanSubject> builder)
    {

        builder.ToTable("StudyPlanSubjects");

        builder.HasKey(sps => sps.Id);

        builder.Property(sps => sps.Id)
            .ValueGeneratedNever();

        builder.Property(sps => sps.TenantId)
            .IsRequired();

        builder.Property(sps => sps.StudyPlanId)
            .IsRequired();

        builder.Property(sps => sps.SubjectId)
            .IsRequired();

        builder.Property(sps => sps.Semester)
            .IsRequired();

        builder.Property(sps => sps.Credits)
            .IsRequired();

        builder.Property(sps => sps.IsRequired)
            .IsRequired();

        builder.Property(sps => sps.Status)
            .IsRequired();

        builder.Property(sps => sps.CreatedAtUtc)
            .IsRequired();

        builder.Property(sps => sps.UpdatedAtUtc);

        builder.HasOne(sps => sps.Subject)
            .WithMany() 
            .HasForeignKey(sps => sps.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sps => new { sps.TenantId, sps.StudyPlanId });
        builder.HasIndex(sps => new { sps.TenantId, sps.SubjectId });

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_StudyPlanSubjects_Semester_Positive",
                "[Semester] > 0");

            tableBuilder.HasCheckConstraint(
                "CK_StudyPlanSubjects_Credits_Positive",
                "[Credits] > 0");
        });
    }
}