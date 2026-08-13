using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class DivisionHeadConfiguration
    : IEntityTypeConfiguration<DivisionHead>
{
    public void Configure(
        EntityTypeBuilder<DivisionHead> builder)
    {
        builder.ToTable("DivisionHeads");

        builder.HasKey(divisionHead => divisionHead.Id);

        builder.Property(divisionHead => divisionHead.Id)
            .HasColumnName("DivisionId")
            .ValueGeneratedNever();

        builder.Property(divisionHead => divisionHead.TenantId)
            .IsRequired();

        builder.Property(divisionHead => divisionHead.ProgramId)
            .HasColumnName("EducationalProgramId")
            .IsRequired();

        builder.Property(divisionHead => divisionHead.PersonId)
            .IsRequired();

        builder.Property(divisionHead => divisionHead.AcademicDegree)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(divisionHead => divisionHead.Status)
            .IsRequired();

        builder.Property(divisionHead => divisionHead.CreatedAtUtc)
            .IsRequired();

        builder.Property(divisionHead => divisionHead.UpdatedAtUtc);

        builder.HasIndex(divisionHead => new
        {
            divisionHead.TenantId,
            divisionHead.ProgramId,
            divisionHead.PersonId
        })
            .IsUnique();

        builder.HasOne<Person>()
            .WithMany()
            .HasForeignKey(divisionHead => divisionHead.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}