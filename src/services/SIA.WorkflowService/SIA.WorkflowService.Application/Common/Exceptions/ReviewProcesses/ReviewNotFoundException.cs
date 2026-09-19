using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.WorkflowService.Application.Common.Exceptions.ReviewProcesses;

public sealed class ReviewNotFoundException : NotFoundException
{
  public ReviewNotFoundException(Guid processId) : base($"No se encontró el proceso de revisión con Id '{processId}'.")
  {
  }
}
