using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
  public void Configure(EntityTypeBuilder<Proposal> builder)
  {
    builder.ToTable("AcademicLoadProposals");

    builder.HasKey(proposal => proposal.Id);
    builder.Property(proposal => proposal.Id).HasColumnName("ProposalId").ValueGeneratedNever();
    builder.Property(proposal => proposal.TenantId).IsRequired();
    builder.Property(proposal => proposal.EducationalProgramId).IsRequired();
    builder.Property(proposal => proposal.AcademicPeriodId).IsRequired();
    builder.Property(proposal => proposal.DivisionHeadId).IsRequired();
    builder.Property(proposal => proposal.ProposalStatus)
        .HasConversion<int>()
        .HasDefaultValue(ProposalStatus.Draft)
        .HasSentinel((ProposalStatus)0)
        .IsRequired();
    builder.Property(proposal => proposal.Status).IsRequired();
    builder.Property(proposal => proposal.CreatedAtUtc).IsRequired();
    builder.Property(proposal => proposal.UpdatedAtUtc);

    builder.HasIndex(proposal => new
    {
      proposal.TenantId,
      proposal.EducationalProgramId,
      proposal.AcademicPeriodId
    }).IsUnique();
  }
}
