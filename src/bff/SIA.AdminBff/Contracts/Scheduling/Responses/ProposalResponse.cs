using SIA.AdminBff.Clients.Scheduling;
using SIA.AdminBff.Contracts.Scheduling.Enums;

namespace SIA.AdminBff.Contracts.Scheduling.Responses;

public sealed record ProposalResponse
{
  public required Guid Id { get; init; }
  public required Guid EducationalProgramId { get; init; }
  public required Guid AcademicPeriodId { get; init; }
  public required Guid DivisionHeadId { get; init; }
  public required ProposalStatus ProposalStatus { get; init; }
  public required bool IsActive { get; init; }
  public required DateTime CreatedAtUtc { get; init; }
  public DateTime? UpdatedAtUtc { get; init; }
  public required Guid CorrelationId { get; init; }
  public static ProposalResponse FromService(ProposalDto serviceResponse)
  {
    return new ProposalResponse
    {
      Id = serviceResponse.Id,
      EducationalProgramId = serviceResponse.EducationalProgramId,
      AcademicPeriodId = serviceResponse.AcademicPeriodId,
      DivisionHeadId = serviceResponse.DivisionHeadId,
      ProposalStatus = serviceResponse.ProposalStatus,
      IsActive = serviceResponse.Status,
      CreatedAtUtc = serviceResponse.CreatedAtUtc,
      UpdatedAtUtc = serviceResponse.UpdatedAtUtc,
      CorrelationId = serviceResponse.CorrelationId
    };
  }
}
