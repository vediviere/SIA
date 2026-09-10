using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Domain.Entities;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.SchedulingService.Infrastructure.Persistence.Contexts;

public sealed class SchedulingDbContext : DbContext
{
  public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : base(options)
  {
  }
  public DbSet<ClassroomLab> ClassroomLabs => Set<ClassroomLab>();

  public DbSet<Building> Buildings => Set<Building>();
  public DbSet<AcademicLoad> AcademicLoad => Set<AcademicLoad>();
  public DbSet<AcademicOffering> AcademicOfferings => Set<AcademicOffering>();
  public DbSet<Group> Groups => Set<Group>();
  public DbSet<TeachingSupportHour> TeachingSupportHours => Set<TeachingSupportHour>();


  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


  public DbSet<ClassroomType> ClassroomTypes => Set<ClassroomType>();
  public DbSet<SupportActivity> SupportActivities => Set<SupportActivity>();
  public DbSet<SupportSchedule> SupportSchedules => Set<SupportSchedule>();
  public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();

  public DbSet<Proposal> AcademicLoadProposals => Set<Proposal>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
  }
}
