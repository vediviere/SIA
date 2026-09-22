using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SIA.TeacherBff.Infrastructure.Http;

namespace SIA.TeacherBff.Tests.Http;

public sealed class ContextHandlerTests
{
  [Fact]
  public async Task SendAsync_AuthenticatedRequest_ForwardsAuthorizationAndCorrelation()
  {
    var correlationId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(
      [
        new Claim(ClaimTypes.NameIdentifier, "teacher")
      ], "Test"))
    };

    context.Request.Headers.Authorization = "Bearer test-token";
    context.Items[CorrelationConstants.ItemKey] = correlationId;

    var accessor = new HttpContextAccessor { HttpContext = context };
    var capture = new CaptureHandler();
    var handler = new RequestContextHandler(accessor, new CorrelationAccessor(accessor))
    {
      InnerHandler = capture
    };

    using var invoker = new HttpMessageInvoker(handler);
    using var request = new HttpRequestMessage(HttpMethod.Get, "https://internal.test/");
    using var response = await invoker.SendAsync(request, CancellationToken.None);

    Assert.Equal("Bearer test-token", capture.Authorization);
    Assert.Equal(correlationId.ToString(), capture.CorrelationId);
  }

  [Fact]
  public async Task SendAsync_UnauthenticatedRequest_DoesNotForwardAuthorization()
  {
    var correlationId = Guid.NewGuid();
    var context = new DefaultHttpContext();

    context.Request.Headers.Authorization = "Bearer untrusted-token";
    context.Items[CorrelationConstants.ItemKey] = correlationId;

    var accessor = new HttpContextAccessor { HttpContext = context };
    var capture = new CaptureHandler();
    var handler = new RequestContextHandler(accessor, new CorrelationAccessor(accessor))
    {
      InnerHandler = capture
    };

    using var invoker = new HttpMessageInvoker(handler);
    using var request = new HttpRequestMessage(HttpMethod.Get, "https://internal.test/");
    using var response = await invoker.SendAsync(request, CancellationToken.None);

    Assert.Null(capture.Authorization);
    Assert.Equal(correlationId.ToString(), capture.CorrelationId);
  }

  private sealed class CaptureHandler : HttpMessageHandler
  {
    public string? Authorization { get; private set; }
    public string? CorrelationId { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      Authorization = request.Headers.Authorization?.ToString();
      CorrelationId = request.Headers.TryGetValues(CorrelationConstants.HeaderName, out var values)
          ? values.SingleOrDefault()
          : null;

      return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
  }
}
