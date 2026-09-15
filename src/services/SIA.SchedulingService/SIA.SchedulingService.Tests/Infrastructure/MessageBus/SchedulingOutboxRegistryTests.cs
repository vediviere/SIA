using System.Reflection;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Infrastructure.MessageBus;

namespace SIA.SchedulingService.Tests.Infrastructure.MessageBus;

public sealed class SchedulingOutboxRegistryTests
{
  [Fact]
  public void Create_ShouldRegisterEveryDefinedIntegrationEvent()
  {
    var registry = SchedulingOutboxRegistry.Create();

    var eventNames = typeof(SchedulingIntegrationEventTypes)
      .GetFields(BindingFlags.Public | BindingFlags.Static)
      .Where(field => field.IsLiteral && field.FieldType == typeof(string))
      .Select(field => (string)field.GetRawConstantValue()!)
      .ToList();

    var registeredTypes = eventNames
      .Select(registry.Resolve)
      .Distinct()
      .OrderBy(type => type.FullName)
      .ToList();

    var definedTypes = typeof(AcademicLoadCreatedIntegrationEvent).Assembly
      .GetTypes()
      .Where(type =>
        type.IsClass &&
        type.Namespace?.StartsWith("SIA.SchedulingService.Contracts.IntegrationEvents", StringComparison.Ordinal) == true &&
        type.Name.EndsWith("IntegrationEvent", StringComparison.Ordinal))
      .OrderBy(type => type.FullName)
      .ToList();

    Assert.Equal(definedTypes, registeredTypes);
  }

  [Fact]
  public void Create_ShouldResolveAcademicLoadUpdatedEvent()
  {
    var registry = SchedulingOutboxRegistry.Create();

    var eventType = registry.Resolve(SchedulingIntegrationEventTypes.AcademicLoadUpdatedV1);

    Assert.Equal(typeof(AcademicLoadUpdatedIntegrationEvent), eventType);
  }
}
