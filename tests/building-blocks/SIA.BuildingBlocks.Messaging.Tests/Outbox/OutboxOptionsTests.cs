using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.BuildingBlocks.Messaging.Tests.Outbox;

public sealed class OutboxOptionsTests
{
  [Fact]
  public void GetRetryDelay_AppliesExponentialBackoff()
  {
    var options = new OutboxOptions
    {
      BaseRetryDelay = TimeSpan.FromSeconds(5),
      MaxRetryDelay = TimeSpan.FromMinutes(5)
    };

    Assert.Equal(TimeSpan.FromSeconds(5), options.GetRetryDelay(1));
    Assert.Equal(TimeSpan.FromSeconds(10), options.GetRetryDelay(2));
    Assert.Equal(TimeSpan.FromSeconds(20), options.GetRetryDelay(3));
    Assert.Equal(TimeSpan.FromSeconds(40), options.GetRetryDelay(4));
  }

  [Fact]
  public void GetRetryDelay_DoesNotExceedMaximum()
  {
    var options = new OutboxOptions
    {
      BaseRetryDelay = TimeSpan.FromSeconds(5),
      MaxRetryDelay = TimeSpan.FromMinutes(5)
    };

    Assert.Equal(TimeSpan.FromMinutes(5), options.GetRetryDelay(7));
    Assert.Equal(TimeSpan.FromMinutes(5), options.GetRetryDelay(20));
  }

  [Fact]
  public void Validate_RejectsInvalidPollingInterval()
  {
    var options = new OutboxOptions
    {
      PollingInterval = TimeSpan.Zero
    };

    Assert.Throws<InvalidOperationException>(() => options.Validate());
  }

  [Fact]
  public void Validate_RejectsInvalidBatchSize()
  {
    var options = new OutboxOptions
    {
      BatchSize = 0
    };

    Assert.Throws<InvalidOperationException>(() => options.Validate());
  }

  [Fact]
  public void Validate_RejectsInvalidRetryConfiguration()
  {
    var options = new OutboxOptions
    {
      BaseRetryDelay = TimeSpan.FromMinutes(2),
      MaxRetryDelay = TimeSpan.FromMinutes(1)
    };

    Assert.Throws<InvalidOperationException>(() => options.Validate());
  }
}
