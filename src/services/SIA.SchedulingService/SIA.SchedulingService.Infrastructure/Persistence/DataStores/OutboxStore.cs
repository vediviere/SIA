using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class OutboxStore : IOutboxStore
{
    private readonly SchedulingDbContext _dbContext;

    public OutboxStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(DateTime utcNow, int batchSize, CancellationToken cancellationToken)
    {
        return await _dbContext.OutboxMessages
            .Where(message => message.ProcessedAtUtc == null
                && message.DeadLetteredAtUtc == null
                && (message.NextAttemptAtUtc == null || message.NextAttemptAtUtc <= utcNow))
            .OrderBy(message => message.RetryCount)
            .ThenBy(message => message.OccurredAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}