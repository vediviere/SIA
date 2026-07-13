using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.Contexts;

public sealed class AcademicDbContext : DbContext
{
  public AcademicDbContext(
      DbContextOptions<AcademicDbContext> options)
      : base(options)
  {
  }

  public DbSet<Subject> Subjects => Set<Subject>();

  public DbSet<OutboxMessage> OutboxMessages =>
      Set<OutboxMessage>();

  protected override void OnModelCreating(
      ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(AcademicDbContext).Assembly);
  }
}
