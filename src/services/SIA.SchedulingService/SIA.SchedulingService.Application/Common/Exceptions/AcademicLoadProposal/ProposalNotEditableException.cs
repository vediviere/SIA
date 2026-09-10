using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;

public sealed class ProposalNotEditableException : ConflictException
{
  public ProposalNotEditableException(Guid proposalId)
    : base($"La propuesta de carga académica con Id '{proposalId}' solo puede modificarse mientras esté activa y en estado Borrador.")
  {
  }
}
