using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchoolControlService.Domain.Entities;
using System.Runtime.Serialization;

namespace SIA.SchoolControlService.Infrastructure.Persistence.Configurations;

public sealed class SubjectReferenceConfiguration
    : IEntityTypeConfiguration<SubjectReference>
{
  public void Configure(
      EntityTypeBuilder<SubjectReference> builder)
  {
    builder.ToTable("SubjectReferences");

    builder.HasKey(subject => subject.Id);

    builder.Property(subject => subject.Id)
        .ValueGeneratedNever();

    builder.Property(subject => subject.SubjectId)
        .IsRequired();

    builder.Property(subject => subject.TenantId)
        .IsRequired();

    builder.Property(subject => subject.Code)
        .HasMaxLength(30)
        .IsRequired();

    builder.Property(subject => subject.Name)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(subject => subject.Credits)
        .IsRequired();

    builder.Property(subject => subject.Status)
        .HasMaxLength(30)
        .IsRequired();

    builder.Property(subject => subject.CreatedAtUtc)
        .IsRequired();

    builder.Property(subject => subject.UpdatedAtUtc)
        .IsRequired();

    builder.HasIndex(subject => subject.SubjectId)
        .IsUnique();

    builder.HasIndex(subject => new
    {
      subject.TenantId,
      subject.Code
    })
    .IsUnique();

    builder.ToTable(tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
          "CK_SubjectReferences_Credits_Positive",
          "[Credits] > 0");
    });
  }
}
