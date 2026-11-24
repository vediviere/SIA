using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Inbox;
using SIA.ReportingService.Infrastructure.Persistence.Contexts;


namespace SIA.ReportingService.Infrastructure.Persistence.DataStores;

public sealed class InboxStore : IInboxStore
{
    private readonly ReportingDbContext _dbContext;

    public InboxStore(ReportingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InboxMessage?> GetAsync(Guid messageId, string consumerName, CancellationToken cancellationToken)
    {
        return await _dbContext.InboxMessages
          .FirstOrDefaultAsync(
            message => message.MessageId == messageId && message.ConsumerName == consumerName,
            cancellationToken);
    }

    public async Task AddAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        await _dbContext.InboxMessages.AddAsync(message, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}