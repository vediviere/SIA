using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Configurations;
public sealed class StudyPlansConfiguration : IEntityTypeConfiguration<StudyPlans>
{
    public void Configure(EntityTypeBuilder<StudyPlans> builder)
    {
        builder.ToTable("StudyPlans");
        builder.HasKey(StudyPlans => StudyPlans.Id);
        builder.Property(StudyPlans => StudyPlans.Id).HasColumnName("StudyPlanId").ValueGeneratedNever();
        builder.Property(StudyPlans => StudyPlans.TenantId).IsRequired();
        builder.Property(StudyPlans => StudyPlans.EducationalProgramId).IsRequired();
        builder.Property(StudyPlans => StudyPlans.Code).HasMaxLength(30).IsRequired();
        builder.Property(StudyPlans => StudyPlans.Name).HasMaxLength(100).IsRequired();
        builder.Property(StudyPlans => StudyPlans.Version).HasMaxLength(50).IsRequired();
        builder.Property(StudyPlans => StudyPlans.EffectiveFrom).IsRequired();
        builder.Property(StudyPlans => StudyPlans.Status).IsRequired();
        builder.Property(StudyPlans => StudyPlans.CreatedAtUtc).IsRequired();
        builder.Property(StudyPlans => StudyPlans.UpdatedAtUtc);

        builder.HasOne<EducationalPrograms>().WithMany().HasForeignKey(StudyPlans => StudyPlans.EducationalProgramId).IsRequired();

        builder.HasIndex(StudyPlans => new
        {
            StudyPlans.TenantId,
            StudyPlans.Code
        })
            .IsUnique();
    }
}
