using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Entities;
using System.Runtime.Serialization;

namespace SIA.SchoolControlService.Infrastructure.Persistence.Contexts;

public sealed class SchoolControlDbContext : DbContext
{
  public SchoolControlDbContext(
      DbContextOptions<SchoolControlDbContext> options)
      : base(options)
  {
  }

  public DbSet<SubjectReference> SubjectReferences =>
      Set<SubjectReference>();

  public DbSet<InboxMessage> InboxMessages =>
      Set<InboxMessage>();

  protected override void OnModelCreating(
      ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(SchoolControlDbContext).Assembly);
  }
}
