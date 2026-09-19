using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchoolControlService.Application.Common.Exceptions.Students;

public sealed class StudentNotFoundException : NotFoundException
{
  public StudentNotFoundException(string studentNumber)
    : base($"No se encontró un estudiante con la matrícula {studentNumber} para esta institución.")
  {
  }
}
