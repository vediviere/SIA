using System.Net.Http.Headers;

namespace SIA.AdminBff.Infrastructure.Http;

public sealed class RequestContextPropagationHandler : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ICorrelationIdAccessor _correlationIdAccessor;

  public RequestContextPropagationHandler(IHttpContextAccessor httpContextAccessor, ICorrelationIdAccessor correlationIdAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
    _correlationIdAccessor = correlationIdAccessor;
  }

  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var httpContext = _httpContextAccessor.HttpContext;
    var authorizationValue = httpContext?.Request.Headers.Authorization.FirstOrDefault();

    if (httpContext?.User.Identity?.IsAuthenticated == true &&
        AuthenticationHeaderValue.TryParse(authorizationValue, out var authorizationHeader))
    {
      request.Headers.Authorization = authorizationHeader;
    }

    request.Headers.Remove(CorrelationIdConstants.HeaderName);
    request.Headers.TryAddWithoutValidation(CorrelationIdConstants.HeaderName, _correlationIdAccessor.CorrelationId.ToString());

    return base.SendAsync(request, cancellationToken);
  }
}
