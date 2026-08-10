using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Infrastructure.Persistence.Contexts;

public sealed class SchedulingDbContext : DbContext
{
    public SchedulingDbContext(
        DbContextOptions<SchedulingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<ClassroomType> ClassroomTypes => Set<ClassroomType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}
