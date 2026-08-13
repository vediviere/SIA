using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class CoordinatorConfiguration
    : IEntityTypeConfiguration<Coordinator>
{
    public void Configure(
        EntityTypeBuilder<Coordinator> builder)
    {
        builder.ToTable("Coordinators");

        builder.HasKey(coordinator => coordinator.Id);

        builder.Property(coordinator => coordinator.Id)
            .HasColumnName("CoordinadorId")
            .ValueGeneratedNever();

        builder.Property(coordinator => coordinator.TenantId)
            .IsRequired();

        builder.Property(coordinator => coordinator.PersonId)
            .IsRequired();

        builder.Property(coordinator => coordinator.AcademicDegree)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(coordinator => coordinator.Status)
            .IsRequired();

        builder.Property(coordinator => coordinator.CreatedAtUtc)
            .IsRequired();

        builder.Property(coordinator => coordinator.UpdatedAtUtc);

        builder.HasIndex(coordinator => new
        {
            coordinator.TenantId,
            coordinator.PersonId
        })
            .IsUnique();

        builder.HasOne<Person>()
            .WithMany()
            .HasForeignKey(coordinator => coordinator.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}