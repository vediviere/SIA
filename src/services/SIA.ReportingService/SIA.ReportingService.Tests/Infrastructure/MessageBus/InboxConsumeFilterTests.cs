using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIA.BuildingBlocks.Messaging.Inbox;
using SIA.ReportingService.Infrastructure.Persistence.Contexts;
using SIA.ReportingService.Infrastructure.Persistence.DataStores;
using SIA.ReportingService.Tests.Common.Fakes;

namespace SIA.ReportingService.Tests.Infrastructure.MessageBus;

public sealed class InboxConsumeFilterTests
{
    [Fact]
    public async Task Send_WithNewMessage_ShouldProcessAndMarkInboxAsProcessed()
    {
        var inboxStore = new FakeInboxStore();
        var tracker = new ExecutionTracker();
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        await using var provider = CreateServiceProvider(inboxStore, tracker);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await harness.Bus.Publish(new SampleIntegrationEvent(tenantId, correlationId), ctx => ctx.MessageId = messageId);

        Assert.True(await harness.Consumed.Any<SampleIntegrationEvent>());
        Assert.Equal(1, tracker.Count);
        Assert.NotNull(inboxStore.AddedMessage);
        Assert.True(inboxStore.AddedMessage.IsProcessed);
        Assert.Equal(tenantId, inboxStore.AddedMessage.TenantId);
        Assert.Equal(correlationId, inboxStore.AddedMessage.CorrelationId);
    }

    [Fact]
    public async Task Send_WhenMessageAlreadyProcessed_ShouldSkipConsumerExecution()
    {
        var inboxStore = new FakeInboxStore();
        var tracker = new ExecutionTracker();
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        await using var provider = CreateServiceProvider(inboxStore, tracker);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        await harness.Bus.Publish(new SampleIntegrationEvent(tenantId, correlationId), ctx => ctx.MessageId = messageId);
        Assert.True(await harness.Consumed.Any<SampleIntegrationEvent>());

        await harness.Bus.Publish(new SampleIntegrationEvent(tenantId, correlationId), ctx => ctx.MessageId = messageId);
        await harness.InactivityTask;

        Assert.Equal(1, tracker.Count);
    }

    [Fact]
    public async Task Send_WhenConsumerThrows_ShouldRecordErrorInInboxMessage()
    {
        var inboxStore = new FakeInboxStore();
        var tracker = new ExecutionTracker();
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        await using var provider = CreateServiceProvider(inboxStore, tracker);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await harness.Bus.Publish(new SampleIntegrationEvent(tenantId, correlationId, ShouldFail: true), ctx => ctx.MessageId = messageId);

        Assert.True(await harness.Consumed.Any<SampleIntegrationEvent>());
        Assert.Equal(0, tracker.Count);
        Assert.NotNull(inboxStore.AddedMessage);
        Assert.False(inboxStore.AddedMessage.IsProcessed);
        Assert.Equal("Error simulado en el consumidor.", inboxStore.AddedMessage.Error);
    }

    private static ServiceProvider CreateServiceProvider(FakeInboxStore inboxStore, ExecutionTracker tracker)
    {
        return new ServiceCollection()
          .AddLogging()
          .AddSingleton<IInboxStore>(inboxStore)
          .AddSingleton(tracker)
          .AddMassTransitTestHarness(cfg =>
          {
              cfg.AddConsumer<SampleConsumer>();
              cfg.UsingInMemory((context, bus) =>
              {
                  bus.UseConsumeFilter(typeof(InboxConsumeFilter<>), context);
                  bus.ConfigureEndpoints(context);
              });
          })
          .BuildServiceProvider(true);
    }

    private sealed record SampleIntegrationEvent(Guid TenantId, Guid CorrelationId, bool ShouldFail = false);

    private sealed class ExecutionTracker
    {
        public int Count { get; private set; }

        public void Increment() => Count++;
    }

    private sealed class SampleConsumer(ExecutionTracker tracker) : IConsumer<SampleIntegrationEvent>
    {
        public Task Consume(ConsumeContext<SampleIntegrationEvent> context)
        {
            if (context.Message.ShouldFail)
            {
                throw new InvalidOperationException("Error simulado en el consumidor.");
            }

            tracker.Increment();
            return Task.CompletedTask;
        }
    }
}