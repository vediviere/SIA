namespace SIA.BuildingBlocks.Messaging.Outbox;

public interface IOutboxEventPublisher
{
  Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken);
}
