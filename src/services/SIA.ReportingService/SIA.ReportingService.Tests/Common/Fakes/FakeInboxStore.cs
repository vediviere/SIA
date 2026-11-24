using SIA.BuildingBlocks.Messaging.Inbox;

namespace SIA.ReportingService.Tests.Common.Fakes;

public sealed class FakeInboxStore : IInboxStore
{
    private readonly Dictionary<(Guid MessageId, string ConsumerName), InboxMessage> _messages = new();

    public InboxMessage? AddedMessage { get; private set; }

    public Task<InboxMessage?> GetAsync(Guid messageId, string consumerName, CancellationToken cancellationToken)
    {
        _messages.TryGetValue((messageId, consumerName), out var message);
        return Task.FromResult(message);
    }

    public Task AddAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        AddedMessage = message;
        _messages[(message.MessageId, message.ConsumerName)] = message;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}