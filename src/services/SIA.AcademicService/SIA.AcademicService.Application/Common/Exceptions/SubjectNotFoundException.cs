namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class SubjectNotFoundException : NotFoundException
{
  public SubjectNotFoundException(Guid subjectId)
      : base($"No se encontró la asignatura con Id {subjectId}.")
  {
  }
}
