using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.WorkflowService.Application.Common.Exceptions.ReviewProcesses;

public sealed class AlreadyDecidedException : ConflictException
{
  public AlreadyDecidedException(Guid processId) : base($"El proceso de revisión con Id '{processId}' ya fue finalizado.")
  {
  }
}
