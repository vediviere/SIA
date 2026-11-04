using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Configurations;

public sealed class TeacherCompetencyConfiguration : IEntityTypeConfiguration<TeacherCompetency>
{
    public void Configure(EntityTypeBuilder<TeacherCompetency> builder)
    {
        builder.ToTable("TeacherCompetencies");

        builder.HasKey(tc => tc.TeacherCompetencyId);

        builder.Property(tc => tc.TeacherCompetencyId)
            .ValueGeneratedNever();

        builder.Property(tc => tc.TenantId)
            .IsRequired();

        builder.Property(tc => tc.TeacherId)
            .IsRequired();

        builder.Property(tc => tc.CompetencyId)
            .IsRequired();

        builder.Property(tc => tc.Level)
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(tc => tc.Stage)
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(tc => tc.Status)
            .IsRequired();

        builder.Property(tc => tc.ValidatedBy);

        builder.Property(tc => tc.ValidateAt);

        builder.Property(tc => tc.CreatedAtUtc)
            .IsRequired();

        builder.Property(tc => tc.UpdatedAtUtc);

        builder.HasIndex(tc => new { tc.TenantId, tc.TeacherId, tc.CompetencyId })
            .IsUnique();

        builder.HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(tc => tc.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}