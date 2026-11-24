using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Inbox;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.ReportingService.Infrastructure.Persistence.Contexts;

public sealed class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options)
    {
    }
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportingDbContext).Assembly);
    }
}
