namespace SIA.AdminBff.Clients.Scheduling.Dtos.Proposal;

public sealed record ProposalCreateDto
{
  public required Guid EducationalProgramId { get; init; }

  public required Guid AcademicPeriodId { get; init; }

  public required Guid DivisionHeadId { get; init; }
}
