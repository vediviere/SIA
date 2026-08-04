namespace SIA.AcademicService.Contracts.IntegrationEvents.Subjects;

public sealed record SubjectCreatedIntegrationEvent
{
  public required Guid EventId { get; init; }

  public required Guid CorrelationId { get; init; }

  public required DateTime OccurredAtUtc { get; init; }

  public required Guid TenantId { get; init; }

  public required Guid SubjectId { get; init; }

  public required string Code { get; init; }

  public required string Name { get; init; }

  public required int Semester { get; init; }

  public required int TheoryHours { get; init; }

  public required int PracticeHours { get; init; }

  public required int Credits { get; init; }

  public required bool Status { get; init; }

  public int Version { get; init; } = 1;
}
