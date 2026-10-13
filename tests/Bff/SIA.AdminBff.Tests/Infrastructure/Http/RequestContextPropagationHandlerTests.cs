using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SIA.AdminBff.Infrastructure.Http;

namespace SIA.AdminBff.Tests.Infrastructure.Http;

public sealed class RequestContextPropagationHandlerTests
{
    [Fact]
    public async Task SendAsync_WithAuthenticatedUser_ShouldPropagateBearerToken()
    {
        var correlationId = Guid.NewGuid();
        var httpContext = CreateHttpContext("Bearer test-token", true);
        var recordingHandler = new RecordingHandler();
        using var client = CreateClient(httpContext, correlationId, recordingHandler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://scheduling.test/api/AcademicLoad");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Bearer", recordingHandler.Authorization?.Scheme);
        Assert.Equal("test-token", recordingHandler.Authorization?.Parameter);
        Assert.Equal(correlationId.ToString(), recordingHandler.CorrelationId);
    }

    [Fact]
    public async Task SendAsync_WithUnauthenticatedUser_ShouldNotPropagateBearerToken()
    {
        var correlationId = Guid.NewGuid();
        var httpContext = CreateHttpContext("Bearer test-token", false);
        var recordingHandler = new RecordingHandler();
        using var client = CreateClient(httpContext, correlationId, recordingHandler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://scheduling.test/api/AcademicLoad");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(recordingHandler.Authorization);
        Assert.Equal(correlationId.ToString(), recordingHandler.CorrelationId);
    }

    [Fact]
    public async Task SendAsync_ShouldReplaceExistingCorrelationId()
    {
        var correlationId = Guid.NewGuid();
        var existingCorrelationId = Guid.NewGuid();
        var httpContext = CreateHttpContext("Bearer test-token", true);
        var recordingHandler = new RecordingHandler();
        using var client = CreateClient(httpContext, correlationId, recordingHandler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://scheduling.test/api/AcademicLoad");

        request.Headers.TryAddWithoutValidation(
            CorrelationIdConstants.HeaderName,
            existingCorrelationId.ToString());

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(correlationId.ToString(), recordingHandler.CorrelationId);
        Assert.NotEqual(existingCorrelationId.ToString(), recordingHandler.CorrelationId);
    }

    private static HttpClient CreateClient(
        HttpContext httpContext,
        Guid correlationId,
        RecordingHandler recordingHandler)
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var correlationIdAccessor = new TestCorrelationIdAccessor(correlationId);

        var propagationHandler = new RequestContextPropagationHandler(
            httpContextAccessor,
            correlationIdAccessor)
        {
            InnerHandler = recordingHandler
        };

        return new HttpClient(propagationHandler);
    }

    private static HttpContext CreateHttpContext(
        string authorization,
        bool isAuthenticated)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = authorization;

        var identity = new ClaimsIdentity(
            isAuthenticated
                ? new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }
                : Array.Empty<Claim>(),
            isAuthenticated ? "TestAuthentication" : null);

        context.User = new ClaimsPrincipal(identity);

        return context;
    }

    private sealed class TestCorrelationIdAccessor : ICorrelationIdAccessor
    {
        public TestCorrelationIdAccessor(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public AuthenticationHeaderValue? Authorization { get; private set; }

        public string? CorrelationId { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Authorization = request.Headers.Authorization;

            if (request.Headers.TryGetValues(
                CorrelationIdConstants.HeaderName,
                out var values))
            {
                CorrelationId = values.Single();
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}