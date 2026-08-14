namespace SIA.BuildingBlocks.Messaging.Outbox;

public sealed class OutboxEventRegistry
{
  private readonly Dictionary<string, Type> _eventTypes = new(StringComparer.Ordinal);

  public OutboxEventRegistry Register<TEvent>(string eventType) where TEvent : class
  {
    if (string.IsNullOrWhiteSpace(eventType))
    {
      throw new ArgumentException("El tipo de evento es obligatorio.", nameof(eventType));
    }

    var normalizedEventType = eventType.Trim();

    if (!_eventTypes.TryAdd(normalizedEventType, typeof(TEvent)))
    {
      throw new InvalidOperationException($"El tipo de evento {normalizedEventType} ya está registrado.");
    }

    return this;
  }

  public Type Resolve(string eventType)
  {
    var normalizedEventType = eventType.Trim();

    if (_eventTypes.TryGetValue(normalizedEventType, out var type))
    {
      return type;
    }

    throw new NotSupportedException($"El tipo de evento {eventType} no está soportado.");
  }
}
