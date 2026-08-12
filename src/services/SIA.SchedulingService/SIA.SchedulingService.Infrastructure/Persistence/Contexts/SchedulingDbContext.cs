using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;
using System.Reflection.Emit;

namespace SIA.SchedulingService.Infrastructure.Persistence.Contexts;

public sealed class SchedulingDbContext : DbContext
{
    public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options): base(options)
    {
    }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}   