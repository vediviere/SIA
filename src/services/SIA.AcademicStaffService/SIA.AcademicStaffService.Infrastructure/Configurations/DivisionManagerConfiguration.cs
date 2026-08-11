using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class DivisionManagerConfiguration
    : IEntityTypeConfiguration<DivisionManager>
{
    public void Configure(
        EntityTypeBuilder<DivisionManager> builder)
    {
        builder.ToTable("DivisionManagers");

        builder.HasKey(divisionManager => divisionManager.Id);

        builder.Property(divisionManager => divisionManager.Id)
            .HasColumnName("DivisionManagerId")
            .ValueGeneratedNever();

        builder.Property(divisionManager => divisionManager.TenantId)
            .IsRequired();

        builder.Property(divisionManager => divisionManager.ProgramId)
            .IsRequired();

        builder.Property(divisionManager => divisionManager.PersonId)
            .IsRequired();

        builder.Property(divisionManager => divisionManager.AcademicDegree)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(divisionManager => divisionManager.Status)
            .IsRequired();

        builder.Property(divisionManager => divisionManager.CreatedAtUtc)
            .IsRequired();

        builder.Property(divisionManager => divisionManager.UpdatedAtUtc);

        builder.HasIndex(divisionManager => new
        {
            divisionManager.TenantId,
            divisionManager.ProgramId,
            divisionManager.PersonId
        })
            .IsUnique();
    }
}