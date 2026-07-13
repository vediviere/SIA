using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration
    : IEntityTypeConfiguration<OutboxMessage>
{
  public void Configure(
      EntityTypeBuilder<OutboxMessage> builder)
  {
    builder.ToTable("OutboxMessages");

    builder.HasKey(message => message.Id);

    builder.Property(message => message.Id)
        .ValueGeneratedNever();

    builder.Property(message => message.EventType)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(message => message.Payload)
        .HasColumnType("nvarchar(max)")
        .IsRequired();

    builder.Property(message => message.OccurredAtUtc)
        .IsRequired();

    builder.Property(message => message.ProcessedAtUtc);

    builder.Property(message => message.RetryCount)
        .IsRequired();

    builder.Property(message => message.Error)
        .HasColumnType("nvarchar(max)");

    builder.Property(message => message.CorrelationId)
        .IsRequired();

    builder.HasIndex(message => message.ProcessedAtUtc);

    builder.HasIndex(message => message.CorrelationId);
  }
}
