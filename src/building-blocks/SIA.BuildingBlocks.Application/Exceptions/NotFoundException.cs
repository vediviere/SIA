namespace SIA.BuildingBlocks.Application.Exceptions;

public abstract class NotFoundException : Exception
{
  protected NotFoundException(string message) : base(message)
  {
  }
}
