using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Configurations;

public sealed class PersonConfiguration
    : IEntityTypeConfiguration<Person>
{
    public void Configure(
        EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");

        builder.HasKey(person => person.Id);

        builder.Property(person => person.Id)
            .HasColumnName("PersonId")
            .ValueGeneratedNever();

        builder.Property(person => person.TenantId)
            .IsRequired();

        builder.Property(person => person.EmployeeNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(person => person.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(person => person.PaternalLastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(person => person.MaternalLastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(person => person.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(person => person.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(person => person.Status)
            .IsRequired();

        builder.Property(person => person.CreatedAtUtc)
            .IsRequired();

        builder.Property(person => person.UpdatedAtUtc);

        builder.HasIndex(person => person.EmployeeNumber)
            .IsUnique();
    }
}