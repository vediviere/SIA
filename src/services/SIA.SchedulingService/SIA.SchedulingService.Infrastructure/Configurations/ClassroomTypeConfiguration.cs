using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class ClassroomTypeConfiguration : IEntityTypeConfiguration<ClassroomType>
{
    public void Configure(EntityTypeBuilder<ClassroomType> builder)
    {
        builder.ToTable("ClassroomTypes");

        builder.HasKey(classroomType => classroomType.Id);

        builder.Property(classroomType => classroomType.Id)
            .HasColumnName("ClassroomTypeId")
            .ValueGeneratedNever();

        builder.Property(classroomType => classroomType.TenantId)
            .IsRequired();

        builder.Property(classroomType => classroomType.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(classroomType => classroomType.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(classroomType => classroomType.Description)
            .HasMaxLength(500);

        builder.Property(classroomType => classroomType.Status)
            .IsRequired();

        builder.Property(classroomType => classroomType.CreatedAtUtc)
            .IsRequired();

        builder.Property(classroomType => classroomType.UpdatedAtUtc);

        builder.HasIndex(classroomType => new
        {
            classroomType.TenantId,
            classroomType.Name
        })
            .IsUnique();
    }
}