using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(group => group.Id);
        builder.Property(group => group.Id).HasColumnName("GroupId").ValueGeneratedNever();

        builder.Property(group => group.TenantId).IsRequired();
        builder.Property(group => group.EducationalProgramId).IsRequired();

        builder.Property(group => group.GroupName).HasMaxLength(200).IsRequired();
        builder.Property(group => group.Shift).HasMaxLength(30).IsRequired();
        builder.Property(group => group.Capacity).IsRequired();

        builder.Property(group => group.Status).IsRequired();
        builder.Property(group => group.CreatedAtUtc).IsRequired();
        builder.Property(group => group.UpdatedAtUtc);

        builder.HasIndex(group => new
        {
            group.TenantId,
            group.EducationalProgramId,
            group.Shift,
            group.GroupName
        })
            .IsUnique();
    }
}