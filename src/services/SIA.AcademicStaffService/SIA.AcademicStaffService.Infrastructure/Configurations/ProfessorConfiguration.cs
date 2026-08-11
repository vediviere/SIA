using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class ProfessorConfiguration
    : IEntityTypeConfiguration<Professor>
{
    public void Configure(
        EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professors");

        builder.HasKey(professor => professor.Id);

        builder.Property(professor => professor.Id)
            .HasColumnName("ProfessorId")
            .ValueGeneratedNever();

        builder.Property(professor => professor.TenantId)
            .IsRequired();

        builder.Property(professor => professor.PersonId)
            .IsRequired();

        builder.Property(professor => professor.AcademicDegree)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(professor => professor.ProfessionalProfile)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(professor => professor.ContractType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(professor => professor.ContractHours)
            .IsRequired();

        builder.Property(professor => professor.Status)
            .IsRequired();

        builder.Property(professor => professor.CreatedAtUtc)
            .IsRequired();

        builder.Property(professor => professor.UpdatedAtUtc);

        builder.HasIndex(professor => new
        {
            professor.TenantId,
            professor.PersonId
        })
            .IsUnique();

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Professors_ContractHours_Positive",
                "[ContractHours] > 0");
        });
    }
}