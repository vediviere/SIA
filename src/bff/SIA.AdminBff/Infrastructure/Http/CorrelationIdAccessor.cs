namespace SIA.AdminBff.Infrastructure.Http;

public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid CorrelationId
  {
    get
    {
      var httpContext = _httpContextAccessor.HttpContext;

      if (httpContext?.Items[CorrelationIdConstants.ItemKey] is not Guid correlationId)
      {
        throw new InvalidOperationException("No existe un CorrelationId para la solicitud actual.");
      }

      return correlationId;
    }
  }
}
