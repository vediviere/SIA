using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.WorkflowService.Domain.Entities;

namespace SIA.WorkflowService.Infrastructure.Persistence.Configurations;

public sealed class ReviewProcessConfig : IEntityTypeConfiguration<ReviewProcess>
{
  public void Configure(EntityTypeBuilder<ReviewProcess> builder)
  {
    builder.ToTable("ReviewProcesses", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint("CK_ReviewProcesses_Version_Positive", "[Version] > 0");
    });

    builder.HasKey(process => process.Id);

    builder.Property(process => process.Id).ValueGeneratedNever();
    builder.Property(process => process.TenantId).IsRequired();
    builder.Property(process => process.AcademicLoadProposalId).IsRequired();
    builder.Property(process => process.DivisionHeadId).IsRequired();
    builder.Property(process => process.Version).IsRequired();
    builder.Property(process => process.Status).HasConversion<int>().IsRequired();
    builder.Property(process => process.SubmittedAtUtc).IsRequired();
    builder.Property(process => process.CreatedAtUtc).IsRequired();
    builder.Property(process => process.UpdatedAtUtc);
    builder.Property(process => process.CorrelationId).IsRequired();

    builder.HasIndex(process => new
    {
      process.TenantId,
      process.AcademicLoadProposalId,
      process.Version
    }).IsUnique();

    builder.HasIndex(process => new
    {
      process.TenantId,
      process.Status
    });

    builder.HasIndex(process => process.CorrelationId);
  }
}
