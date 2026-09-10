using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;

public sealed class ProposalAlreadyExistsException : ConflictException
{
  public ProposalAlreadyExistsException(Guid educationalProgramId, Guid academicPeriodId)
    : base($"Ya existe una propuesta de carga académica para el programa educativo con ID '{educationalProgramId}' y el periodo académico con ID '{academicPeriodId}'.")
  {
  }
}
