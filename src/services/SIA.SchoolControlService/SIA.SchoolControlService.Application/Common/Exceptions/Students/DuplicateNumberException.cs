using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchoolControlService.Application.Common.Exceptions.Students;

public sealed class DuplicateNumberException : ConflictException
{
  public DuplicateNumberException(string studentNumber)
    : base($"Ya existe un estudiante con la matrícula {studentNumber} para esta institución.")
  {
  }
}
