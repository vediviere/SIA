namespace SIA.BuildingBlocks.Application.Exceptions;

public abstract class ConflictException : Exception
{
  protected ConflictException(string message) : base(message)
  {
  }
}
