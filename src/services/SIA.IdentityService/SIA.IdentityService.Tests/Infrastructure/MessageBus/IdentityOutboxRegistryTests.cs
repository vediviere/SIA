using System.Reflection;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Infrastructure.MessageBus;

namespace SIA.IdentityService.Tests.Infrastructure.MessageBus;

public sealed class IdentityOutboxRegistryTests
{
  [Fact]
  public void Create_ShouldRegisterEveryDefinedIntegrationEvent()
  {
    var registry = IdentityOutboxRegistry.Create();

    var eventNames = typeof(UserIntegrationEventTypes)
      .GetFields(BindingFlags.Public | BindingFlags.Static)
      .Where(field => field.IsLiteral && field.FieldType == typeof(string))
      .Select(field => (string)field.GetRawConstantValue()!)
      .ToList();

    var registeredTypes = eventNames
      .Select(registry.Resolve)
      .Distinct()
      .OrderBy(type => type.FullName)
      .ToList();

    var definedTypes = typeof(UserCreatedIntegrationEvent).Assembly
      .GetTypes()
      .Where(type =>
        type.IsClass &&
        type.Namespace?.StartsWith("SIA.IdentityService.Contracts.IntegrationEvents", StringComparison.Ordinal) == true &&
        type.Name.EndsWith("IntegrationEvent", StringComparison.Ordinal))
      .OrderBy(type => type.FullName)
      .ToList();

    Assert.Equal(definedTypes, registeredTypes);
  }
}
