namespace SIA.BuildingBlocks.Messaging.Inbox;

public interface IInboxStore
{
    Task<InboxMessage?> GetAsync(Guid messageId, string consumerName, CancellationToken cancellationToken);
    Task AddAsync(InboxMessage message, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}