using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;

public sealed class ProposalNotValidForApprovalException : ConflictException
{
    public ProposalNotValidForApprovalException(Guid proposalId)
      : base($"La propuesta de carga académica con Id '{proposalId}' no cuenta con cargas académicas activas y no puede aprobarse.")
    {
    }
}