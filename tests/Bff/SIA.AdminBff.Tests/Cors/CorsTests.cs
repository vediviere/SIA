using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SIA.AdminBff.Tests.Cors;

public sealed class CorsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string AllowedOrigin = "http://localhost:4200";
    private const string DisallowedOrigin = "https://pruebasSIA.com";
    private const string Path = "/"; 

    private readonly HttpClient _client;
    public CorsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Preflight_WithAllowedOrigin_ShouldReturnAllowOrigin()
    {
        var request = CreatePreflight(AllowedOrigin, "GET");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values));
        Assert.Equal(AllowedOrigin, values!.Single());
    }

    [Fact]
    public async Task Preflight_WithDisallowedOrigin_ShouldNotReturnAllowOrigin()
    {
        var request = CreatePreflight(DisallowedOrigin, "GET");
        var response = await _client.SendAsync(request);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Preflight_WithAuthorizationAndCorrelationIdHeaders_ShouldAllowThem()
    {
        var request = CreatePreflight(
            AllowedOrigin,
            "POST",
            "authorization,x-correlation-id,content-type");

        var response = await _client.SendAsync(request);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Headers", out var values));
        var allowedHeaders = string.Join(",", values!).ToLowerInvariant();
        Assert.Contains("authorization", allowedHeaders);
        Assert.Contains("x-correlation-id", allowedHeaders);
    }
    [Fact]
    public async Task Preflight_WithUnknownHeader_ShouldNotAllowThatHeader()
    {
        var request = CreatePreflight(AllowedOrigin, "GET", "x-unknown-header");
        var response = await _client.SendAsync(request);
        if (response.Headers.TryGetValues("Access-Control-Allow-Headers", out var values))
        {
            var allowedHeaders = string.Join(",", values).ToLowerInvariant();
            Assert.DoesNotContain("x-unknown-header", allowedHeaders);
        }
    }
    [Fact]
    public async Task Get_WithAllowedOrigin_ShouldExposeCorrelationIdHeader()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Path);
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(request);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origin));
        Assert.Equal(AllowedOrigin, origin!.Single());
        Assert.True(response.Headers.TryGetValues("Access-Control-Expose-Headers", out var exposed));
        Assert.Contains("X-Correlation-Id", string.Join(",", exposed!), StringComparison.OrdinalIgnoreCase);
    }
    [Fact]
    public async Task Get_WithDisallowedOrigin_ShouldNotReturnCorsHeaders()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Path);
        request.Headers.Add("Origin", DisallowedOrigin);
        var response = await _client.SendAsync(request);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
    [Fact]
    public async Task Get_WithCorrelationId_ShouldReturnSameCorrelationIdInResponse()
    {
        var correlationId = Guid.NewGuid().ToString();
        var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("X-Correlation-Id", correlationId);

        var response = await _client.SendAsync(request);

        Assert.True(response.Headers.TryGetValues("X-Correlation-Id", out var values));
        Assert.Equal(correlationId, values!.Single());
    }
    [Fact]
    public async Task Post_WithInvalidBearerAndAllowedOrigin_ShouldReturnUnauthorizedWithCorsHeaders()
    {
        var proposalId = Guid.NewGuid();
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/academic-planning/proposals/{proposalId}/loads")
        {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Authorization", "Bearer token-invalido");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
    private static HttpRequestMessage CreatePreflight(string origin, string method, string? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, Path);
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", method);

        if (headers is not null)
        {
            request.Headers.Add("Access-Control-Request-Headers", headers);
        }
        return request;
    }
}