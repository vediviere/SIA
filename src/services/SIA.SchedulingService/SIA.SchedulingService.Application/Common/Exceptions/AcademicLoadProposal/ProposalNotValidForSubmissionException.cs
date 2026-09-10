using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
public sealed class ProposalNotValidForSubmissionException : ConflictException
{
    public ProposalNotValidForSubmissionException(Guid proposalId)
        : base($"La propuesta de carga académica con Id '{proposalId}' no cuenta con cargas académicas asociadas y no puede enviarse a revisión.")
    {
    }
}