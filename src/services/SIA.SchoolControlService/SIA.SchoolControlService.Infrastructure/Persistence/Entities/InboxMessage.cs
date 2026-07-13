namespace SIA.SchoolControlService.Infrastructure.Persistence.Entities;

public sealed class InboxMessage
{
  private InboxMessage()
  {
  }

  public InboxMessage(
      Guid id,
      string eventType,
      string sourceService,
      Guid correlationId)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException(
          "El identificador del mensaje es obligatorio.",
          nameof(id));
    }

    if (string.IsNullOrWhiteSpace(eventType))
    {
      throw new ArgumentException(
          "El tipo de evento es obligatorio.",
          nameof(eventType));
    }

    if (string.IsNullOrWhiteSpace(sourceService))
    {
      throw new ArgumentException(
          "El servicio de origen es obligatorio.",
          nameof(sourceService));
    }

    Id = id;
    EventType = eventType.Trim();
    SourceService = sourceService.Trim();
    CorrelationId = correlationId;
    ReceivedAtUtc = DateTime.UtcNow;
    RetryCount = 0;
  }

  public Guid Id { get; private set; }

  public string EventType { get; private set; } = string.Empty;

  public string SourceService { get; private set; } = string.Empty;

  public DateTime ReceivedAtUtc { get; private set; }

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
