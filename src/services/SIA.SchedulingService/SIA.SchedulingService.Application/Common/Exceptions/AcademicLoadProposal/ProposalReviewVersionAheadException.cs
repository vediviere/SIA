using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;

public sealed class ProposalReviewVersionAheadException : ConflictException
{
    public ProposalReviewVersionAheadException(Guid proposalId, int receivedVersion, int currentVersion)
      : base($"La decisión recibida corresponde a la versión {receivedVersion} de la propuesta '{proposalId}', pero la versión vigente es {currentVersion}.")
    {
    }
}