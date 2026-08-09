using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Domain.Entities;
using System.Reflection.Emit;

namespace SIA.SchedulingService.Infrastructure.Persistence.Contexts;

public sealed class SchedulingDbContext : DbContext
{
    public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options): base(options)
    {
    }

    public DbSet<Building> Buildings => Set<Building>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}