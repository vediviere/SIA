namespace SIA.BuildingBlocks.Messaging.Inbox;

public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    public InboxMessage(Guid messageId, string consumerName, string eventType, Guid tenantId, Guid correlationId)
    {
        if (messageId == Guid.Empty)
        {
            throw new ArgumentException("El identificador del mensaje es obligatorio.", nameof(messageId));
        }

        if (string.IsNullOrWhiteSpace(consumerName))
        {
            throw new ArgumentException("El nombre del consumidor es obligatorio.", nameof(consumerName));
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("El tipo de evento es obligatorio.", nameof(eventType));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El identificador del tenant es obligatorio.", nameof(tenantId));
        }

        if (correlationId == Guid.Empty)
        {
            throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
        }

        MessageId = messageId;
        ConsumerName = consumerName.Trim();
        EventType = eventType.Trim();
        TenantId = tenantId;
        CorrelationId = correlationId;
        ReceivedAtUtc = DateTime.UtcNow;
    }

    public Guid MessageId { get; private set; }
    public string ConsumerName { get; private set; } = string.Empty;
    public string EventType { get; private set; } = string.Empty;
    public Guid TenantId { get; private set; }
    public Guid CorrelationId { get; private set; }
    public DateTime ReceivedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public string? Error { get; private set; }

    public bool IsProcessed => ProcessedAtUtc.HasValue;

    public void MarkAsProcessed()
    {
        ProcessedAtUtc = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
    }
}