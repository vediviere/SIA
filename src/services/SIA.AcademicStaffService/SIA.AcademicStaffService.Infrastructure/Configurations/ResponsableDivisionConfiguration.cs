using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class ResponsableDivisionConfiguration
    : IEntityTypeConfiguration<ResponsableDivision>
{
    public void Configure(
        EntityTypeBuilder<ResponsableDivision> builder)
    {
        builder.ToTable("ResponsableDivision");

        builder.HasKey(responsable => responsable.Id);

        builder.Property(responsable => responsable.Id)
            .HasColumnName("DivisionId")
            .ValueGeneratedNever();

        builder.Property(responsable => responsable.TenantId)
            .IsRequired();

        builder.Property(responsable => responsable.ProgramaId)
            .IsRequired();

        builder.Property(responsable => responsable.PersonaId)
            .IsRequired();

        builder.Property(responsable => responsable.GradoAcademico)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(responsable => responsable.Status)
            .IsRequired();

        builder.Property(responsable => responsable.CreatedAtUtc)
            .IsRequired();

        builder.Property(responsable => responsable.UpdatedAtUtc);

        builder.HasIndex(responsable => new
        {
            responsable.TenantId,
            responsable.ProgramaId,
            responsable.PersonaId
        })
            .IsUnique();
    }
}