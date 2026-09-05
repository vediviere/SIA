namespace SIA.AdminBff.Contracts.Scheduling.Requests;

public sealed record CreateSupportHoursRequest
{
  public required Guid ActivityId { get; init; }
  public required int Hours { get; init; }
}
