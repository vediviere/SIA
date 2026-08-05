namespace SIA.AcademicService.Application.Common.Exceptions;

public abstract class ConflictException : Exception
{
  protected ConflictException(string message)
      : base(message)
  {
  }
}
