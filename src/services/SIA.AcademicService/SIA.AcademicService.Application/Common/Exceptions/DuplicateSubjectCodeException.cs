using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class DuplicateSubjectCodeException : ConflictException
{
  public DuplicateSubjectCodeException(string code)
      : base($"Ya existe una asignatura con el código {code} para esta institución.")
  {
  }
}
