using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Configurations;
public sealed class EducationalProgramsConfiguration : IEntityTypeConfiguration<EducationalProgram>
{
    public void Configure(EntityTypeBuilder<EducationalProgram> builder)
    {
        builder.ToTable("EducationalPrograms");
        builder.HasKey(EducationalPrograms => EducationalPrograms.Id);
        builder.Property(EducationalPrograms => EducationalPrograms.Id).HasColumnName("EducationalProgramId").ValueGeneratedNever();
        builder.Property(EducationalPrograms => EducationalPrograms.TenantId).IsRequired();
        builder.Property(EducationalPrograms => EducationalPrograms.Code).HasMaxLength(30).IsRequired();
        builder.Property(EducationalPrograms => EducationalPrograms.Name).HasMaxLength(100).IsRequired();
        builder.Property(EducationalPrograms => EducationalPrograms.Level).HasMaxLength(50).IsRequired();
        builder.Property(EducationalPrograms => EducationalPrograms.Status).IsRequired();
        builder.Property(EducationalPrograms => EducationalPrograms.CreatedAtUtc).IsRequired();
        builder.Property(EducationalPrograms => EducationalPrograms.UpdatedAtUtc);

        builder.HasIndex(EducationalProgram => new
        {
            EducationalProgram.TenantId,
            EducationalProgram.Code
        })
            .IsUnique();

    }
}