using SIA.AdminBff.Clients.Scheduling;

namespace SIA.AdminBff.Contracts.Scheduling.Responses;

public sealed record SupportHourResponse
{
  public required Guid Id { get; init; }
  public required Guid ActivityId { get; init; }
  public required Guid AcademicLoadId { get; init; }
  public required int Hours { get; init; }
  public required bool IsActive { get; init; }
  public required DateTime CreatedAtUtc { get; init; }
  public DateTime? UpdatedAtUtc { get; init; }
  public required Guid CorrelationId { get; init; }
  public static SupportHourResponse FromDto(SupportHourDto dto)
  {
    return new SupportHourResponse
    {
      Id = dto.Id,
      ActivityId = dto.ActivityId,
      AcademicLoadId = dto.AcademicLoadId,
      Hours = dto.Hours,
      IsActive = dto.Status,
      CreatedAtUtc = dto.CreatedAtUtc,
      UpdatedAtUtc = dto.UpdatedAtUtc,
      CorrelationId = dto.CorrelationId
    };
  }
}
