using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.BuildingBlocks.Messaging.Inbox;

namespace SIA.ReportingService.Infrastructure.Configurations;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(message => new { message.MessageId, message.ConsumerName });

        builder.Property(message => message.ConsumerName)
          .HasMaxLength(200)
          .IsRequired();

        builder.Property(message => message.EventType)
          .HasMaxLength(200)
          .IsRequired();

        builder.Property(message => message.TenantId)
          .IsRequired();

        builder.Property(message => message.CorrelationId)
          .IsRequired();

        builder.Property(message => message.ReceivedAtUtc)
          .IsRequired();

        builder.Property(message => message.ProcessedAtUtc);
        builder.Property(message => message.Error);

        builder.HasIndex(message => message.TenantId);
        builder.HasIndex(message => message.CorrelationId);
        builder.HasIndex(message => message.ProcessedAtUtc);
    }
}