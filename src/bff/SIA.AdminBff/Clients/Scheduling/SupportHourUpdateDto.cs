namespace SIA.AdminBff.Clients.Scheduling;

public sealed record SupportHourUpdateDto
{
  public required int Hours { get; init; }
}
