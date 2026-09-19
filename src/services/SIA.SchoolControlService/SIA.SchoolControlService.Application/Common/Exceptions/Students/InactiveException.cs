using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchoolControlService.Application.Common.Exceptions.Students;

public sealed class InactiveException : ConflictException
{
  public InactiveException(string studentNumber)
    : base($"El estudiante con la matrícula {studentNumber} no se encuentra activo.")
  {
  }
}
