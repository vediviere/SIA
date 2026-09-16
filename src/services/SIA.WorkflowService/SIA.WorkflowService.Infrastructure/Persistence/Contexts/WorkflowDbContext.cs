using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Infrastructure.Persistence.Entities;

namespace SIA.WorkflowService.Infrastructure.Persistence.Contexts;

public sealed class WorkflowDbContext : DbContext
{
  public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
  {
  }

  public DbSet<ReviewProcess> ReviewProcesses => Set<ReviewProcess>();
  public DbSet<ReviewObservation> ReviewObservations => Set<ReviewObservation>();
  public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
  }
}
