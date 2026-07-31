using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Configurations;

public sealed class SubjectConfiguration
    : IEntityTypeConfiguration<Subject>
{
  public void Configure(
      EntityTypeBuilder<Subject> builder)
  {
    builder.ToTable("Subjects");

    builder.HasKey(subject => subject.Id);

    builder.Property(subject => subject.Id)
        .HasColumnName("SubjectId")
        .ValueGeneratedNever();

    builder.Property(subject => subject.TenantId)
        .IsRequired();

    builder.Property(subject => subject.StudyPlanId)
         .IsRequired();

    builder.Property(subject => subject.Code)
        .HasMaxLength(30)
        .IsRequired();

    builder.Property(subject => subject.Name)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(subject => subject.Semester)
        .IsRequired();

    builder.Property(subject => subject.Credits)
        .IsRequired();

    builder.Property(subject => subject.Status)
        .IsRequired();

    builder.Property(subject => subject.CreatedAtUtc)
        .IsRequired();

    builder.Property(subject => subject.UpdatedAtUtc);

    builder.HasIndex(subject => new
    {
      subject.TenantId,
      subject.Code
    })
        .IsUnique();

    builder.ToTable(tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
          "CK_Subjects_Credits_Positive",
          "[Credits] > 0");
      tableBuilder.HasCheckConstraint(
          "CK_Subjects_Semester_Positive",
          "[Semester] > 0");

      tableBuilder.HasCheckConstraint(
          "CK_Subjects_TheoryHours_NonNegative",
          "[TheoryHours] >= 0");

      tableBuilder.HasCheckConstraint(
          "CK_Subjects_PracticeHours_NonNegative",
          "[PracticeHours] >= 0");
    });
  }
}
