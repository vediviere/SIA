using SIA.AdminBff.Clients.Scheduling;

namespace SIA.AdminBff.Contracts.Scheduling.Responses;

public sealed record LoadResponse
{
  public required Guid Id { get; init; }
  public required Guid ProposalId { get; init; }
  public required Guid TeacherId { get; init; }
  public required Guid DivisionId { get; init; }
  public required Guid AcademicPeriodId { get; init; }
  public required string OfficialLetterNumber { get; init; }
  public required DateTime ProposedDate { get; init; }
  public required int ClassHours { get; init; }
  public required int SupportHours { get; init; }
  public required DateTime AssignmentDate { get; init; }
  public required bool IsActive { get; init; }
  public required DateTime CreatedAtUtc { get; init; }
  public DateTime? UpdatedAtUtc { get; init; }
  public required Guid CorrelationId { get; init; }
  public static LoadResponse FromDto(LoadDto dto)
  {
    return new LoadResponse
    {
      Id = dto.Id,
      ProposalId = dto.ProposalId,
      TeacherId = dto.TeacherId,
      DivisionId = dto.DivisionId,
      AcademicPeriodId = dto.AcademicPeriodId,
      OfficialLetterNumber = dto.OfficialLetterNumber,
      ProposedDate = dto.ProposedDate,
      ClassHours = dto.ClassHours,
      SupportHours = dto.SupportHours,
      AssignmentDate = dto.AssignmentDate,
      IsActive = dto.Status,
      CreatedAtUtc = dto.CreatedAtUtc,
      UpdatedAtUtc = dto.UpdatedAtUtc,
      CorrelationId = dto.CorrelationId
    };
  }
}
