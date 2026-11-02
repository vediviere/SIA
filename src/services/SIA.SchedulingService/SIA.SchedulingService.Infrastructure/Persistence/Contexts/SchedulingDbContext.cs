using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Entities.Academics;
using SIA.SchedulingService.Domain.Entities.Facilities;
using SIA.SchedulingService.Domain.Entities.Schedules;
using SIA.SchedulingService.Domain.Entities.TeachingSupport;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;
using OutboxMessage = SIA.BuildingBlocks.Messaging.Outbox.OutboxMessage;

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


    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


    public DbSet<ClassroomType> ClassroomTypes => Set<ClassroomType>();
    public DbSet<SupportActivity> SupportActivities => Set<SupportActivity>();
    public DbSet<SupportSchedule> SupportSchedules => Set<SupportSchedule>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();

    public DbSet<Proposal> AcademicLoadProposals => Set<Proposal>();

    public DbSet<AcademicLoadGroup> AcademicLoadGroups => Set<AcademicLoadGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}
