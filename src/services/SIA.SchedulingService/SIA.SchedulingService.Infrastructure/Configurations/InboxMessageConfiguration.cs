using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;

namespace SIA.SchedulingService.Infrastructure.Persistence.Configurations;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).ValueGeneratedNever();

        builder.Property(message => message.EventType).HasMaxLength(200).IsRequired();
        builder.Property(message => message.SourceService).HasMaxLength(200).IsRequired();
        builder.Property(message => message.ReceivedAtUtc).IsRequired();
        builder.Property(message => message.ProcessedAtUtc);
        builder.Property(message => message.RetryCount).IsRequired();
        builder.Property(message => message.Error).HasMaxLength(2000);
        builder.Property(message => message.CorrelationId).IsRequired();
    }
}