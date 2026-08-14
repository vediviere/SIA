namespace SIA.BuildingBlocks.Messaging.Outbox;

public sealed class OutboxOptions
{
  public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
  public int BatchSize { get; set; } = 20;
  public int MaxRetryCount { get; set; } = 5;
  public TimeSpan BaseRetryDelay { get; set; } = TimeSpan.FromSeconds(5);
  public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromMinutes(5);

  public TimeSpan GetRetryDelay(int retryNumber)
  {
    var multiplier = Math.Pow(2, Math.Max(0, retryNumber - 1));
    var milliseconds = BaseRetryDelay.TotalMilliseconds * multiplier;
    return TimeSpan.FromMilliseconds(Math.Min(milliseconds, MaxRetryDelay.TotalMilliseconds));
  }

  public void Validate()
  {
    if (PollingInterval <= TimeSpan.Zero)
    {
      throw new InvalidOperationException("PollingInterval debe ser mayor que cero.");
    }

    if (BatchSize <= 0)
    {
      throw new InvalidOperationException("BatchSize debe ser mayor que cero.");
    }

    if (MaxRetryCount <= 0)
    {
      throw new InvalidOperationException("MaxRetryCount debe ser mayor que cero.");
    }

    if (BaseRetryDelay <= TimeSpan.Zero || MaxRetryDelay < BaseRetryDelay)
    {
      throw new InvalidOperationException("La configuración de reintentos del Outbox no es válida.");
    }
  }
}
