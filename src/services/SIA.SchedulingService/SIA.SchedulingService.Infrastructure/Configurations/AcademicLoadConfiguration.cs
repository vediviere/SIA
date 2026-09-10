
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class AcademicLoadConfiguration : IEntityTypeConfiguration<AcademicLoad>
{
  public void Configure(EntityTypeBuilder<AcademicLoad> builder)
  {
    builder.ToTable("AcademicLoad");

    builder.Property(load => load.Id).HasColumnName("AcademicLoadId").ValueGeneratedNever();

    builder.Property(load => load.TenantId).IsRequired();
    builder.Property(load => load.ProposalId).IsRequired();
    builder.Property(load => load.TeacherId).IsRequired();
    builder.HasOne<Proposal>().WithMany().HasForeignKey(load => load.ProposalId).OnDelete(DeleteBehavior.Restrict);
    builder.Property(load => load.DivisionId).IsRequired();
    builder.Property(load => load.AcademicPeriodId).IsRequired();
    builder.Property(load => load.OfficialLetterNumber).HasMaxLength(100).IsRequired();
    builder.Property(load => load.ProposedDate).IsRequired();
    builder.Property(load => load.ClassHours).IsRequired();
    builder.Property(load => load.SupportHours).IsRequired();
    builder.Property(load => load.AssignmentDate).IsRequired();
    builder.Property(load => load.Status).IsRequired();
    builder.Property(load => load.CreatedAtUtc).IsRequired();
    builder.Property(load => load.UpdatedAtUtc);
    builder.HasIndex(load => new { load.TenantId, load.TeacherId, load.AcademicPeriodId });
  }
}
