using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.WorkflowService.Domain.Entities;

namespace SIA.WorkflowService.Infrastructure.Persistence.Configurations;

public sealed class ReviewObservationConfig : IEntityTypeConfiguration<ReviewObservation>
{
  public void Configure(EntityTypeBuilder<ReviewObservation> builder)
  {
    builder.ToTable("ReviewObservations");

    builder.HasKey(observation => observation.Id);

    builder.Property(observation => observation.Id).HasColumnName("ReviewObservationId").ValueGeneratedNever();
    builder.Property(observation => observation.ReviewProcessId).IsRequired();
    builder.Property(observation => observation.TenantId).IsRequired();
    builder.Property(observation => observation.TargetType).HasConversion<int>().IsRequired();
    builder.Property(observation => observation.TargetId).IsRequired();
    builder.Property(observation => observation.Description).IsRequired();
    builder.Property(observation => observation.CreatedBy).IsRequired();
    builder.Property(observation => observation.CreatedAtUtc).IsRequired();
    builder.Property(observation => observation.CorrelationId).IsRequired();

    builder.HasOne<ReviewProcess>()
      .WithMany(process => process.Observations)
      .HasForeignKey(observation => observation.ReviewProcessId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(observation => new
    {
      observation.TenantId,
      observation.ReviewProcessId
    });

    builder.HasIndex(observation => new
    {
      observation.TargetType,
      observation.TargetId
    });

    builder.HasIndex(observation => observation.CorrelationId);
  }
}
