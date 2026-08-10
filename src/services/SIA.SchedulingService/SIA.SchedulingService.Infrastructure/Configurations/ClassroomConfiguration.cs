using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class ClassroomConfiguration : IEntityTypeConfiguration<Classroom>
{
    public void Configure(EntityTypeBuilder<Classroom> builder)
    {
        builder.ToTable("Classrooms");
        builder.HasKey(classroom => classroom.Id);

        builder.Property(classroom => classroom.Id)
            .HasColumnName("ClassroomId")
            .ValueGeneratedNever();

        builder.Property(classroom => classroom.TenantId)
            .IsRequired();

        builder.Property(classroom => classroom.BuildingId)
            .IsRequired();

        builder.Property(classroom => classroom.ClassroomTypeId)
            .IsRequired();

        builder.Property(classroom => classroom.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(classroom => classroom.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(classroom => classroom.Capacity)
            .IsRequired();

        builder.Property(classroom => classroom.Description)
            .HasMaxLength(500);

        builder.Property(classroom => classroom.Status)
            .IsRequired();

        builder.Property(classroom => classroom.CreatedAtUtc)
            .IsRequired();

        builder.Property(classroom => classroom.UpdatedAtUtc);

        builder.HasOne(classroom => classroom.ClassroomType)
            .WithMany()
            .HasForeignKey(classroom => classroom.ClassroomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(classroom => new
        {
            classroom.TenantId,
            classroom.Code
        })
            .IsUnique();

        builder.HasIndex(classroom => classroom.ClassroomTypeId);
        builder.HasIndex(classroom => classroom.BuildingId);

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Classrooms_Capacity_Positive",
                "[Capacity] > 0");
        });
    }
}