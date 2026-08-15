using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.BuildingBlocks.Messaging.Tests.Outbox;

public sealed class OutboxMessageTests
{
  [Fact]
  public void Constructor_CreatesMessage()
  {
    var correlationId = Guid.NewGuid();

    var message = new OutboxMessage("TestEvent.v1", "{}", correlationId);

    Assert.NotEqual(Guid.Empty, message.Id);
    Assert.Equal("TestEvent.v1", message.EventType);
    Assert.Equal("{}", message.Payload);
    Assert.Equal(correlationId, message.CorrelationId);
    Assert.Equal(0, message.RetryCount);
    Assert.Null(message.ProcessedAtUtc);
    Assert.Null(message.DeadLetteredAtUtc);
  }

  [Fact]
  public void MarkAsDeadLettered_QuarantinesMessage()
  {
    var message = new OutboxMessage("TestEvent.v1", "{}", Guid.NewGuid());

    message.MarkAsFailed("Error 1", DateTime.UtcNow.AddSeconds(5));
    message.MarkAsFailed("Error 2", DateTime.UtcNow.AddSeconds(10));
    message.MarkAsFailed("Error 3", DateTime.UtcNow.AddSeconds(20));
    message.MarkAsFailed("Error 4", DateTime.UtcNow.AddSeconds(40));
    message.MarkAsDeadLettered("Error permanente");

    Assert.Equal(5, message.RetryCount);
    Assert.Equal("Error permanente", message.Error);
    Assert.NotNull(message.LastAttemptAtUtc);
    Assert.NotNull(message.DeadLetteredAtUtc);
    Assert.Null(message.NextAttemptAtUtc);
    Assert.Null(message.ProcessedAtUtc);
    Assert.True(message.IsDeadLettered);
  }
}
