namespace SIA.AdminBff.Infrastructure.Http;

public sealed class CorrelationIdMiddleware
{
  private readonly RequestDelegate _next;

  public CorrelationIdMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var correlationId = ResolveCorrelationId(context.Request);
    context.Items[CorrelationIdConstants.ItemKey] = correlationId;
    context.TraceIdentifier = correlationId.ToString();

    context.Response.OnStarting(() =>
    {
      context.Response.Headers[CorrelationIdConstants.HeaderName] = correlationId.ToString();
      return Task.CompletedTask;
    });

    await _next(context);
  }

  private static Guid ResolveCorrelationId(HttpRequest request)
  {
    if (request.Headers.TryGetValue(CorrelationIdConstants.HeaderName, out var headerValue) &&
        Guid.TryParse(headerValue.FirstOrDefault(), out var correlationId))
    {
      return correlationId;
    }

    return Guid.NewGuid();
  }
}
