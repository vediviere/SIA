namespace SIA.AdminBff.Clients.Scheduling;

public sealed record ProposalCreateDto
{
  public required Guid TenantId { get; init; }

  public required Guid EducationalProgramId { get; init; }

  public required Guid AcademicPeriodId { get; init; }

  public required Guid DivisionHeadId { get; init; }
}
