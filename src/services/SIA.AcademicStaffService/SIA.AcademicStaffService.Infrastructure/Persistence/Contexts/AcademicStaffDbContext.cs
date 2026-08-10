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

    public DbSet<Docente> Docentes => Set<Docente>();

    public DbSet<ResponsableDivision> ResponsablesDivision => Set<ResponsableDivision>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicStaffDbContext).Assembly);
    }
}