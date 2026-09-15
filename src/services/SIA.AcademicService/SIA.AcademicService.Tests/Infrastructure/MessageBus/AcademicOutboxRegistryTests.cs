using System.Reflection;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Infrastructure.MessageBus;

namespace SIA.AcademicService.Tests.Infrastructure.MessageBus;

public sealed class AcademicOutboxRegistryTests
{
  [Fact]
  public void Create_ShouldRegisterEveryDefinedIntegrationEvent()
  {
    var registry = AcademicOutboxRegistry.Create();

    var eventNames = typeof(AcademicIntegrationEventTypes)
      .GetFields(BindingFlags.Public | BindingFlags.Static)
      .Where(field => field.IsLiteral && field.FieldType == typeof(string))
      .Select(field => (string)field.GetRawConstantValue()!)
      .ToList();

    var registeredTypes = eventNames
      .Select(registry.Resolve)
      .Distinct()
      .OrderBy(type => type.FullName)
      .ToList();

    var definedTypes = typeof(SubjectCreatedIntegrationEvent).Assembly
      .GetTypes()
      .Where(type =>
        type.IsClass &&
        type.Namespace?.StartsWith("SIA.AcademicService.Contracts.IntegrationEvents", StringComparison.Ordinal) == true &&
        type.Name.EndsWith("IntegrationEvent", StringComparison.Ordinal))
      .OrderBy(type => type.FullName)
      .ToList();

    Assert.Equal(definedTypes, registeredTypes);
  }
}
