using System.Text.Json;
using MassTransit;

namespace SIA.BuildingBlocks.Messaging.Outbox;

public sealed class MassTransitOutboxEventPublisher : IOutboxEventPublisher
{
  private readonly IPublishEndpoint _publishEndpoint;
  private readonly OutboxEventRegistry _eventRegistry;

  public MassTransitOutboxEventPublisher(IPublishEndpoint publishEndpoint, OutboxEventRegistry eventRegistry)
  {
    _publishEndpoint = publishEndpoint;
    _eventRegistry = eventRegistry;
  }

  public async Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken)
  {
    var messageType = _eventRegistry.Resolve(eventType);
    var message = JsonSerializer.Deserialize(payload, messageType)
      ?? throw new InvalidOperationException($"No fue posible deserializar el evento {eventType}.");

    await _publishEndpoint.Publish(message, messageType, cancellationToken);
  }
}
