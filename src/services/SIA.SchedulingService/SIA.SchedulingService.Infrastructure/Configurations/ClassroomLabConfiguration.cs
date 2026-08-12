using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class ClassroomLabConfiguration : IEntityTypeConfiguration<ClassroomLab>
{
    public void Configure(EntityTypeBuilder<ClassroomLab> builder)
    {
        builder.ToTable("ClassroomLabs");
        builder.HasKey(classroomLab => classroomLab.Id);

        builder.Property(classroomLab => classroomLab.Id)
            .HasColumnName("ClassroomLabId")
            .ValueGeneratedNever();

        builder.Property(classroomLab => classroomLab.TenantId)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.BuildingId)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.ClassroomTypeId)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.Capacity)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.Description)
            .HasMaxLength(500);

        builder.Property(classroomLab => classroomLab.Status)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.CreatedAtUtc)
            .IsRequired();

        builder.Property(classroomLab => classroomLab.UpdatedAtUtc);

        builder.HasOne(classroomLab => classroomLab.ClassroomType)
            .WithMany()
            .HasForeignKey(classroomLab => classroomLab.ClassroomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(classroomLab => new
        {
            classroomLab.TenantId,
            classroomLab.Code
        })
            .IsUnique();

        builder.HasIndex(classroomLab => classroomLab.ClassroomTypeId);
        builder.HasIndex(classroomLab => classroomLab.BuildingId);

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Classrooms_Capacity_Positive",
                "[Capacity] > 0");
        });
    }
}