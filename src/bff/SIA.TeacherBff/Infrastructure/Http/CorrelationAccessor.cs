namespace SIA.TeacherBff.Infrastructure.Http;

public sealed class CorrelationAccessor : ICorrelationAccessor
{
  private readonly IHttpContextAccessor _contextAccessor;

  public CorrelationAccessor(IHttpContextAccessor contextAccessor)
  {
    _contextAccessor = contextAccessor;
  }

  public Guid CorrelationId
  {
    get
    {
      if (_contextAccessor.HttpContext?.Items[CorrelationConstants.ItemKey] is not Guid correlationId)
      {
        throw new InvalidOperationException("No existe un CorrelationId para la solicitud actual.");
      }

      return correlationId;
    }
  }
}
