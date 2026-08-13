using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;
using System.Reflection.Emit;


namespace SIA.SchedulingService.Infrastructure.Persistence.Contexts;

public sealed class SchedulingDbContext : DbContext
{
    public SchedulingDbContext(
        DbContextOptions<SchedulingDbContext> options)
        : base(options)
    {
    }
    public DbSet<ClassroomLab> ClassroomLabs => Set<ClassroomLab>();

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<ClassroomType> ClassroomTypes => Set<ClassroomType>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();




    public DbSet<SupportSchedule> SupportSchedules { get; set; }
    public DbSet<ClassSchedule> ClassSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}   