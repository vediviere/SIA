using SIA.AdminBff.Contracts.Scheduling.Enums;

namespace SIA.AdminBff.Clients.Scheduling;

public sealed record ProposalDto
{
  public required Guid Id { get; init; }

  public required Guid TenantId { get; init; }

  public required Guid EducationalProgramId { get; init; }

  public required Guid AcademicPeriodId { get; init; }

  public required Guid DivisionHeadId { get; init; }

  public required ProposalStatus ProposalStatus { get; init; }

  public required bool Status { get; init; }

  public required DateTime CreatedAtUtc { get; init; }

  public DateTime? UpdatedAtUtc { get; init; }

  public required Guid CorrelationId { get; init; }
}
