using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
public sealed class ProposalNotFoundException : NotFoundException
{
    public ProposalNotFoundException(Guid proposalId)
        : base($"No se encontró la propuesta de carga académica con el Id '{proposalId}'.")
    {
    }
}