namespace SIA.SchedulingService.Infrastructure.Persistence.Entities;

public sealed class OutboxMessage
{
    private OutboxMessage() { }

    public OutboxMessage(
        string eventType,
        string payload,
        Guid correlationId)

    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("El tipo de evento es obligatorio.", nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("El contenido del evento es obligatorio.", nameof(payload));
        }

        Id = Guid.NewGuid();
        EventType = eventType.Trim();
        Payload = payload;
        OccurredAtUtc = DateTime.UtcNow;
        CorrelationId = correlationId;
        RetryCount = 0;
    }
    public Guid Id { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }
    public Guid CorrelationId { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedAtUtc = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}
