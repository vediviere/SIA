namespace SIA.AdminBff.Clients.Scheduling.Dtos.SupportHour;

public sealed record SupportHourUpdateDto
{
  public required int Hours { get; init; }
}
