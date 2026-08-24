using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Contracts.IntegrationEvents;

namespace SIA.SchedulingService.Tests.Infrastructure.Outbox;
public sealed class TeachingSupportHoursOutboxRegistrationTest
{
    private static OutboxEventRegistry CreateRegistryTSH()
    {
        return new OutboxEventRegistry()
            .Register<TeachingSupportHoursCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursCreatedV1)
            .Register<TeachingSupportHoursUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursUpdatedV1)
            .Register<TeachingSupportHoursDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursDeactivatedV1)
            .Register<TeachingSupportHoursActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursActivatedV1);
    }
    [Fact]
    public void Resolve_TeachingSupportHoursCreatedV1_ShouldReturnCorrectType()
    {
        var registryTSH = CreateRegistryTSH();
        var resolvedType = registryTSH.Resolve(SchedulingIntegrationEventTypes.TeachingSupportHoursCreatedV1);
        Assert.Equal(typeof(TeachingSupportHoursCreatedIntegrationEvent), resolvedType);
    }
    [Fact]
    public void Resolve_TeachingSupportHoursUpdatedV1_ShouldReturnCorrectType()
    {
        var registryTSH = CreateRegistryTSH();
        var resolvedType = registryTSH.Resolve(SchedulingIntegrationEventTypes.TeachingSupportHoursUpdatedV1);
        Assert.Equal(typeof(TeachingSupportHoursUpdatedIntegrationEvent), resolvedType);
    }
    [Fact]
    public void Resolve_TeachingSupportHoursDeactivatedV1_ShouldReturnCorrectType()
    {
        var registryTSH = CreateRegistryTSH();
        var resolvedType = registryTSH.Resolve(SchedulingIntegrationEventTypes.TeachingSupportHoursDeactivatedV1);
        Assert.Equal(typeof(TeachingSupportHoursDeactivatedIntegrationEvent), resolvedType);
    }
    [Fact]
    public void Resolve_TeachingSupportHoursActivatedV1_ShouldReturnCorrectType()
    {
        var registryTSH = CreateRegistryTSH();
        var resolvedType = registryTSH.Resolve(SchedulingIntegrationEventTypes.TeachingSupportHoursActivatedV1);
        Assert.Equal(typeof(TeachingSupportHoursActivatedIntegrationEvent), resolvedType);
    }

    [Fact]
    public void Resolve_UnknownEventType_ShouldThrowNotSupportedException()
    {
        var registryTSH = CreateRegistryTSH();
        Assert.Throws<NotSupportedException>(() => registryTSH.Resolve("UnknownEvent.v1"));
    }
}