using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

public sealed class AcademicStaffDbContext : DbContext
{
    public AcademicStaffDbContext(
        DbContextOptions<AcademicStaffDbContext> options)
        : base(options)
    {
    }

    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<DivisionHead> DivisionHeads => Set<DivisionHead>();

    public DbSet<Coordinator> Coordinators => Set<Coordinator>();

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicStaffDbContext).Assembly);
    }
}