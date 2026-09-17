using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchoolControlService.Application.Common.Exceptions.Students;

public sealed class LinkedException : ConflictException
{
  public LinkedException(string studentNumber)
    : base($"La matrícula {studentNumber} ya está vinculada con otra cuenta.")
  {
  }
}
