using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

public sealed class AcademicStaffDbContext : DbContext
{
    public AcademicStaffDbContext(
        DbContextOptions<AcademicStaffDbContext> options)
        : base(options)
    {
    }

    public DbSet<Teacher> Teacher => Set<Teacher>();

    public DbSet<DivisionHead> DivisionHead => Set<DivisionHead>();

    public DbSet<OutboxMessage> OutboxMessage => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicStaffDbContext).Assembly);
    }
}