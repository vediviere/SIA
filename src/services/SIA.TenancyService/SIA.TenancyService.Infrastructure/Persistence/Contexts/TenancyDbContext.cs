using Microsoft.EntityFrameworkCore;
using SIA.TenancyService.Domain.Entities;

namespace SIA.TenancyService.Infrastructure.Persistence.Contexts;

public sealed class TenancyDbContext : DbContext
{
  public TenancyDbContext(DbContextOptions<TenancyDbContext> options) : base(options)
  {
  }

  public DbSet<Tenant> Tenants => Set<Tenant>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenancyDbContext).Assembly);
  }
}
