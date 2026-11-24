using MassTransit;
using Microsoft.Extensions.Logging;

namespace SIA.BuildingBlocks.Messaging.Inbox;

public sealed class InboxConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IInboxStore _inboxStore;
    private readonly ILogger<InboxConsumeFilter<T>> _logger;

    public InboxConsumeFilter(IInboxStore inboxStore, ILogger<InboxConsumeFilter<T>> logger)
    {
        _inboxStore = inboxStore;
        _logger = logger;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var messageId = context.MessageId
          ?? throw new InvalidOperationException($"El mensaje {typeof(T).Name} no contiene MessageId.");

        var consumerName = context.ReceiveContext.InputAddress?.AbsolutePath.Trim('/') ?? typeof(T).Name;
        var eventType = typeof(T).FullName ?? typeof(T).Name;

        var correlationId = ResolveCorrelationId(context);
        var tenantId = ResolveTenantId(context);

        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TenantId"] = tenantId,
            ["MessageId"] = messageId
        });

        var existing = await _inboxStore.GetAsync(messageId, consumerName, context.CancellationToken);
        if (existing is { IsProcessed: true })
        {
            _logger.LogInformation("Mensaje duplicado ignorado por Inbox. MessageId: {MessageId}, Consumer: {ConsumerName}", messageId, consumerName);
            return;
        }

        var inboxMessage = existing ?? new InboxMessage(messageId, consumerName, eventType, tenantId, correlationId);
        if (existing is null)
        {
            await _inboxStore.AddAsync(inboxMessage, context.CancellationToken);
        }

        try
        {
            await next.Send(context);

            inboxMessage.MarkAsProcessed();
            await _inboxStore.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception exception)
        {
            inboxMessage.MarkAsFailed(exception.Message);
            await _inboxStore.SaveChangesAsync(context.CancellationToken);
            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("inbox");
    }

    private static Guid ResolveCorrelationId(ConsumeContext<T> context)
    {
        if (context.CorrelationId.HasValue && context.CorrelationId.Value != Guid.Empty)
        {
            return context.CorrelationId.Value;
        }

        if (context.Headers.TryGetHeader("X-Correlation-Id", out var headerValue)
          && Guid.TryParse(headerValue?.ToString(), out var parsedHeader)
          && parsedHeader != Guid.Empty)
        {
            return parsedHeader;
        }

        var prop = typeof(T).GetProperty("CorrelationId")?.GetValue(context.Message);
        if (prop is Guid guidProp && guidProp != Guid.Empty)
        {
            return guidProp;
        }

        return Guid.NewGuid();
    }

    private static Guid ResolveTenantId(ConsumeContext<T> context)
    {
        if (context.Headers.TryGetHeader("X-Tenant-Id", out var headerValue)
          && Guid.TryParse(headerValue?.ToString(), out var parsedHeader)
          && parsedHeader != Guid.Empty)
        {
            return parsedHeader;
        }

        var prop = typeof(T).GetProperty("TenantId")?.GetValue(context.Message);
        if (prop is Guid guidProp && guidProp != Guid.Empty)
        {
            return guidProp;
        }

        throw new InvalidOperationException($"No se pudo resolver TenantId para el evento {typeof(T).Name}.");
    }
}