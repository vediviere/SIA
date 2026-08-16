using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.BuildingBlocks.Messaging.Outbox;


namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
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

        builder.Property(message => message.LastAttemptAtUtc);
        builder.Property(message => message.NextAttemptAtUtc);
        builder.Property(message => message.DeadLetteredAtUtc);

        builder.Property(message => message.RetryCount)
            .IsRequired();

        builder.Property(message => message.Error)
            .HasColumnType("nvarchar(max)");

        builder.Property(message => message.CorrelationId)
            .IsRequired();

        builder.HasIndex(message => new
        {
            message.ProcessedAtUtc,
            message.DeadLetteredAtUtc,
            message.NextAttemptAtUtc
        });

        builder.HasIndex(message => message.CorrelationId);
    }
}