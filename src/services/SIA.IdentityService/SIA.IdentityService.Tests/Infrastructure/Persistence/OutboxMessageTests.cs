using SIA.IdentityService.Infrastructure.Persistence.Entities;

namespace SIA.IdentityService.Tests.Infrastructure.Persistence;

public sealed class OutboxMessageTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateOutboxMessage()
  {
    var correlationId = Guid.NewGuid();

    var message = new OutboxMessage("UserCreatedIntegrationEvent.v1", "{}", correlationId);

    Assert.NotEqual(Guid.Empty, message.Id);
    Assert.Equal("UserCreatedIntegrationEvent.v1", message.EventType);
    Assert.Equal("{}", message.Payload);
    Assert.Equal(correlationId, message.CorrelationId);
    Assert.Equal(0, message.RetryCount);
    Assert.Null(message.ProcessedAtUtc);
    Assert.Null(message.Error);
  }

  [Fact]
  public void Constructor_WithEmptyEventType_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new OutboxMessage("", "{}", Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyPayload_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new OutboxMessage("UserCreatedIntegrationEvent.v1", "", Guid.NewGuid()));
  }

  [Fact]
  public void MarkAsProcessed_ShouldSetProcessedDateAndClearError()
  {
    var message = new OutboxMessage("UserCreatedIntegrationEvent.v1", "{}", Guid.NewGuid());

    message.MarkAsFailed("Error de prueba");

    message.MarkAsProcessed();

    Assert.NotNull(message.ProcessedAtUtc);
    Assert.Null(message.Error);
  }

  [Fact]
  public void MarkAsFailed_ShouldIncrementRetryCountAndStoreError()
  {
    var message = new OutboxMessage("UserCreatedIntegrationEvent.v1", "{}", Guid.NewGuid());

    message.MarkAsFailed("Error de prueba");

    Assert.Equal(1, message.RetryCount);
    Assert.Equal("Error de prueba", message.Error);
  }
}
