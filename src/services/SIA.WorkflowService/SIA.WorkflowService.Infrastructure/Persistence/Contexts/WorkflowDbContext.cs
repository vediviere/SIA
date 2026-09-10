using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Infrastructure.Persistence.Entities;
using System.Reflection.Emit;

namespace SIA.WorkflowService.Infrastructure.Persistence.Contexts;

public sealed class WorkflowDbContext : DbContext
{
  public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
  {
  }

  public DbSet<ReviewProcess> ReviewProcesses => Set<ReviewProcess>();
  public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
  }
}
