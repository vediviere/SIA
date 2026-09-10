using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
  public void Configure(EntityTypeBuilder<Student> builder)
  {
    builder.ToTable("Students");

    builder.HasKey(student => student.Id);

    builder.Property(student => student.Id)
      .HasColumnName("StudentId")
      .ValueGeneratedNever();

    builder.Property(student => student.TenantId)
      .IsRequired();

    builder.Property(student => student.StudentNumber)
      .HasMaxLength(Student.StudentNumberMaxLength)
      .IsRequired();

    builder.Property(student => student.FirstName)
      .HasMaxLength(Student.FirstNameMaxLength)
      .IsRequired();

    builder.Property(student => student.PaternalLastName)
     .HasMaxLength(Student.LastNameMaxLength)
     .IsRequired();

    builder.Property(student => student.MaternalLastName)
      .HasMaxLength(Student.LastNameMaxLength)
      .IsRequired();

    builder.Property(student => student.Status)
      .IsRequired();

    builder.Property(student => student.CreatedAtUtc)
      .IsRequired();

    builder.Property(student => student.UpdatedAtUtc);

    builder.HasIndex(student => new
    {
      student.TenantId,
      student.StudentNumber
    })
    .IsUnique();
  }
}
