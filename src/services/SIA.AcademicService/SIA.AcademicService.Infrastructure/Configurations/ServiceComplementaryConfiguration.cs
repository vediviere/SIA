using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Infrastructure.Configurations;

public sealed class ServiceComplementaryConfiguration : IEntityTypeConfiguration<ServiceComplementary>
{
    public void Configure(EntityTypeBuilder<ServiceComplementary> builder)
    {
        builder.ToTable("ServiceComplementary");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Id)
            .HasColumnName("ComplementaryCreditId")
            .ValueGeneratedNever();

        builder.Property(sc => sc.TenantId)
            .IsRequired();

        builder.Property(sc => sc.StudyPlanId)
            .IsRequired();

        builder.Property(sc => sc.Type)
            .IsRequired();

        builder.Property(sc => sc.Credit)
            .IsRequired();

        builder.Property(sc => sc.Status)
            .IsRequired();

        builder.Property(sc => sc.CreatedAtUtc)
            .IsRequired();

        builder.Property(sc => sc.UpdatedAtUtc);

        builder.HasOne(sc => sc.StudyPlan)
            .WithMany()
            .HasForeignKey(sc => sc.StudyPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_ServiceComplementary_Credit_Positive",
                "[Credit] > 0");
        });
    }
}