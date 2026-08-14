namespace SIA.BuildingBlocks.Messaging.Outbox;

public interface IOutboxStore
{
  Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(DateTime utcNow, int batchSize, CancellationToken cancellationToken);
  Task SaveChangesAsync(CancellationToken cancellationToken);
}
