using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SIA.IdentityService.Infrastructure.Persistence.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
  public void Configure(EntityTypeBuilder<AuditLog> builder)
  {
    builder.ToTable("AuditLogs");

    builder.HasKey(auditLog => auditLog.Id);

    builder.Property(auditLog => auditLog.Id)
        .HasColumnName("AuditLogId")
        .ValueGeneratedNever();

    builder.Property(auditLog => auditLog.TenantId)
        .IsRequired();

    builder.Property(auditLog => auditLog.Action)
        .HasMaxLength(150)
        .IsRequired();

    builder.Property(auditLog => auditLog.EntityName)
        .HasMaxLength(150)
        .IsRequired();

    builder.Property(auditLog => auditLog.EntityId)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(auditLog => auditLog.UserId);

    builder.Property(auditLog => auditLog.OccurredAtUtc)
        .IsRequired();

    builder.Property(auditLog => auditLog.OldValues);

    builder.Property(auditLog => auditLog.NewValues);

    builder.Property(auditLog => auditLog.CorrelationId)
        .IsRequired();

    builder.HasIndex(auditLog => auditLog.TenantId);

    builder.HasIndex(auditLog => auditLog.UserId);

    builder.HasIndex(auditLog => auditLog.CorrelationId);
  }
}
