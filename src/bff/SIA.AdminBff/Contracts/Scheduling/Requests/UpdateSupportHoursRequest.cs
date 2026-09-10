namespace SIA.AdminBff.Contracts.Scheduling.Requests;

public sealed record UpdateSupportHoursRequest
{
  public required int Hours { get; init; }
}
