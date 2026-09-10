
namespace SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;

public sealed class AcademicOfferingCreatedIntegrationEvet
{
  public required Guid EventId { get; init; }
  public required Guid CorrelationId { get; init; }
  public required DateTime OccurredAtUtc { get; init; }
  public required Guid TenantId { get; init; }
  public required Guid OfferingId { get; init; }
  public required Guid GroupId { get; init; }
  public required Guid SubjectId { get; init; }
  public required Guid AcademicLoadId { get; init; }
  public required string OfferingStatus { get; init; }
  public required int ClassHours { get; init; }
  public required bool Status { get; init; }
  public int Version { get; init; } = 1;
}
