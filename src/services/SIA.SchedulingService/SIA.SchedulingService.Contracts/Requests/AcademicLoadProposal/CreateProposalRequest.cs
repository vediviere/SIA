namespace SIA.SchedulingService.Contracts.Requests.AcademicLoadProposal;

public sealed record CreateProposalRequest
{
  public required Guid EducationalProgramId { get; init; }
  public required Guid AcademicPeriodId { get; init; }
  public required Guid DivisionHeadId { get; init; }
}
