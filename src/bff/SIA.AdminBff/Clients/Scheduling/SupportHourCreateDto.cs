namespace SIA.AdminBff.Clients.Scheduling;

public sealed record SupportHourCreateDto
{
  public required Guid TenantId { get; init; }
  public required Guid ActivityId { get; init; }
  public required Guid AcademicLoadId { get; init; }
  public required int Hours { get; init; }
}
