using System.Net.Http.Headers;

namespace SIA.TeacherBff.Infrastructure.Http;

public sealed class RequestContextHandler : DelegatingHandler
{
  private readonly IHttpContextAccessor _contextAccessor;
  private readonly ICorrelationAccessor _correlationAccessor;

  public RequestContextHandler(IHttpContextAccessor contextAccessor, ICorrelationAccessor correlationAccessor)
  {
    _contextAccessor = contextAccessor;
    _correlationAccessor = correlationAccessor;
  }

  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var context = _contextAccessor.HttpContext;
    var value = context?.Request.Headers.Authorization.FirstOrDefault();

    if (context?.User.Identity?.IsAuthenticated == true &&
        AuthenticationHeaderValue.TryParse(value, out var authorization))
    {
      request.Headers.Authorization = authorization;
    }

    request.Headers.Remove(CorrelationConstants.HeaderName);
    request.Headers.TryAddWithoutValidation(CorrelationConstants.HeaderName, _correlationAccessor.CorrelationId.ToString());

    return base.SendAsync(request, cancellationToken);
  }
}
