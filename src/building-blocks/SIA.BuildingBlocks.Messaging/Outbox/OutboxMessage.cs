namespace SIA.BuildingBlocks.Messaging.Outbox;

public sealed class OutboxMessage
{
  private OutboxMessage()
  {
  }

  public OutboxMessage(string eventType, string payload, Guid correlationId)
  {
    if (string.IsNullOrWhiteSpace(eventType))
    {
      throw new ArgumentException("El tipo de evento es obligatorio.", nameof(eventType));
    }

    if (string.IsNullOrWhiteSpace(payload))
    {
      throw new ArgumentException("El contenido del evento es obligatorio.", nameof(payload));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    Id = Guid.NewGuid();
    EventType = eventType.Trim();
    Payload = payload;
    CorrelationId = correlationId;
    OccurredAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public string EventType { get; private set; } = string.Empty;
  public string Payload { get; private set; } = string.Empty;
  public DateTime OccurredAtUtc { get; private set; }
  public DateTime? ProcessedAtUtc { get; private set; }
  public DateTime? LastAttemptAtUtc { get; private set; }
  public DateTime? NextAttemptAtUtc { get; private set; }
  public DateTime? DeadLetteredAtUtc { get; private set; }
  public int RetryCount { get; private set; }
  public string? Error { get; private set; }
  public Guid CorrelationId { get; private set; }
  public bool IsDeadLettered => DeadLetteredAtUtc.HasValue;

  public void MarkAsProcessed()
  {
    var now = DateTime.UtcNow;
    ProcessedAtUtc = now;
    LastAttemptAtUtc = now;
    NextAttemptAtUtc = null;
    DeadLetteredAtUtc = null;
    Error = null;
  }

  public void MarkAsFailed(string error, DateTime nextAttemptAtUtc)
  {
    RetryCount++;
    Error = error;
    LastAttemptAtUtc = DateTime.UtcNow;
    NextAttemptAtUtc = nextAttemptAtUtc;
  }

  public void MarkAsDeadLettered(string error)
  {
    var now = DateTime.UtcNow;
    RetryCount++;
    Error = error;
    LastAttemptAtUtc = now;
    DeadLetteredAtUtc = now;
    NextAttemptAtUtc = null;
  }
}
