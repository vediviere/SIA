using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class DocenteConfiguration
    : IEntityTypeConfiguration<Docente>
{
    public void Configure(
        EntityTypeBuilder<Docente> builder)
    {
        builder.ToTable("Docente");

        builder.HasKey(docente => docente.Id);

        builder.Property(docente => docente.Id)
            .HasColumnName("DocenteId")
            .ValueGeneratedNever();

        builder.Property(docente => docente.TenantId)
            .IsRequired();

        builder.Property(docente => docente.PersonaId)
            .IsRequired();

        builder.Property(docente => docente.GradoAcademico)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(docente => docente.PerfilProfesional)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(docente => docente.TipoContrato)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(docente => docente.HorasContrato)
            .IsRequired();

        builder.Property(docente => docente.Status)
            .IsRequired();

        builder.Property(docente => docente.CreatedAtUtc)
            .IsRequired();

        builder.Property(docente => docente.UpdatedAtUtc);

        builder.HasIndex(docente => new
        {
            docente.TenantId,
            docente.PersonaId
        })
            .IsUnique();

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Docente_HorasContrato_Positive",
                "[HorasContrato] > 0");
        });
    }
}