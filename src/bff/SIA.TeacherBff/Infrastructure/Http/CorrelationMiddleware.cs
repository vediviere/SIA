namespace SIA.TeacherBff.Infrastructure.Http;

public sealed class CorrelationMiddleware
{
  private readonly RequestDelegate _next;

  public CorrelationMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var correlationId = Resolve(context.Request);

    context.Items[CorrelationConstants.ItemKey] = correlationId;
    context.TraceIdentifier = correlationId.ToString();

    context.Response.OnStarting(() =>
    {
      context.Response.Headers[CorrelationConstants.HeaderName] = correlationId.ToString();
      return Task.CompletedTask;
    });

    await _next(context);
  }

  private static Guid Resolve(HttpRequest request)
  {
    if (request.Headers.TryGetValue(CorrelationConstants.HeaderName, out var value) &&
        Guid.TryParse(value.FirstOrDefault(), out var correlationId) &&
        correlationId != Guid.Empty)
    {
      return correlationId;
    }

    return Guid.NewGuid();
  }
}
