using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class TeacherConfiguration
    : IEntityTypeConfiguration<Teacher>
{
    public void Configure(
        EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teacher");

        builder.HasKey(teacher => teacher.Id);

        builder.Property(teacher => teacher.Id)
            .HasColumnName("TeacherId")
            .ValueGeneratedNever();

        builder.Property(teacher => teacher.TenantId)
            .IsRequired();

        builder.Property(teacher => teacher.PersonId)
            .IsRequired();

        builder.Property(teacher => teacher.AcademicDegree)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(teacher => teacher.ProfessionalProfile)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(teacher => teacher.ContractType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(teacher => teacher.ContractHours)
            .IsRequired();

        builder.Property(teacher => teacher.Status)
            .IsRequired();

        builder.Property(teacher => teacher.CreatedAtUtc)
            .IsRequired();

        builder.Property(teacher => teacher.UpdatedAtUtc);

        builder.HasIndex(teacher => new
        {
            teacher.TenantId,
            teacher.PersonId
        })
            .IsUnique();

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Teacher_ContractHours_Positive",
                "[ContractHours] > 0");
        });
    }
}