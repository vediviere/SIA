using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Infrastructure.Configurations;

public sealed class ClassScheduleConfiguration : IEntityTypeConfiguration<ClassSchedule>
{
    public void Configure(EntityTypeBuilder<ClassSchedule> builder)
    {
        builder.ToTable("ClassSchedules"); 

        builder.HasKey(x => x.Id); 

        builder.Property(x => x.Id)
            .HasColumnName("ClassScheduleId")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .IsRequired(); 

        builder.Property(x => x.OfferingId)
            .IsRequired(); 

        builder.Property(x => x.ClassroomLabId)
            .IsRequired();

        builder.Property(x => x.AcademicPeriodId)
            .IsRequired(); 

        builder.Property(x => x.Day)
            .HasColumnType("varchar(20)")
            .IsRequired(); 

        builder.Property(x => x.StartTime)
            .HasColumnType("time")
            .HasConversion(
                dateTime => dateTime.TimeOfDay,
                timeSpan => DateTime.MinValue.Add(timeSpan))
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnType("time")
            .HasConversion(
                dateTime => dateTime.TimeOfDay,
                timeSpan => DateTime.MinValue.Add(timeSpan))
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired(); 

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc); 

        builder.HasOne(x => x.ClassroomLab)
            .WithMany()
            .HasForeignKey(x => x.ClassroomLabId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}