using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SIA.TeacherBff.Configuration;
using SIA.TeacherBff.Infrastructure.Errors;
using SIA.TeacherBff.Tests.Support;

namespace SIA.TeacherBff.Tests.Integration;

public sealed class BaseTests : IClassFixture<BffFactory>
{
  private readonly BffFactory _factory;

  public BaseTests(BffFactory factory)
  {
    _factory = factory;
  }

  [Fact]
  public async Task Health_WithoutToken_ReturnsOk()
  {
    using var client = CreateClient();

    var response = await client.GetAsync("/health");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task Probe_WithoutToken_ReturnsUnauthorized()
  {
    using var client = CreateClient();

    var response = await client.GetAsync("/api/probe");
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    Assert.NotNull(error);
    Assert.Equal("UNAUTHORIZED", error.Code);
    Assert.True(response.Headers.Contains("X-Correlation-Id"));
  }

  [Fact]
  public async Task Probe_WithTokenWithoutTenant_ReturnsUnauthorized()
  {
    using var client = CreateClient();
    using var request = new HttpRequestMessage(HttpMethod.Get, "/api/probe");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _factory.CreateTokenWithoutTenant());

    var response = await client.SendAsync(request);

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task Probe_WithValidToken_ReturnsTenant()
  {
    using var client = CreateClient();
    var tenantId = Guid.NewGuid();
    using var request = new HttpRequestMessage(HttpMethod.Get, "/api/probe");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _factory.CreateToken(tenantId));

    var response = await client.SendAsync(request);
    var context = await response.Content.ReadFromJsonAsync<ContextDto>();

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.NotNull(context);
    Assert.Equal(tenantId, context.TenantId);
  }

  [Fact]
  public async Task Probe_WithCorrelationId_ReturnsSameValue()
  {
    using var client = CreateClient();
    var correlationId = Guid.NewGuid().ToString();
    using var request = new HttpRequestMessage(HttpMethod.Get, "/api/probe");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _factory.CreateToken(Guid.NewGuid()));
    request.Headers.Add("X-Correlation-Id", correlationId);

    var response = await client.SendAsync(request);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());
  }

  [Fact]
  public async Task MissingInternalResource_ReturnsNormalizedError()
  {
    using var client = CreateClient();
    var correlationId = Guid.NewGuid();
    using var request = new HttpRequestMessage(HttpMethod.Get, "/api/probe/missing");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _factory.CreateToken(Guid.NewGuid()));
    request.Headers.Add("X-Correlation-Id", correlationId.ToString());

    var response = await client.SendAsync(request);
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    Assert.NotNull(error);
    Assert.Equal("RESOURCE_NOT_FOUND", error.Code);
    Assert.Equal(correlationId, error.CorrelationId);
  }

  [Fact]
  public void InternalClients_UseConfiguredUrls()
  {
    var factory = _factory.Services.GetRequiredService<IHttpClientFactory>();

    var academic = factory.CreateClient(ServiceConfig.Academic);
    var staff = factory.CreateClient(ServiceConfig.Staff);
    var scheduling = factory.CreateClient(ServiceConfig.Scheduling);

    Assert.Equal("https://academic.test/", academic.BaseAddress?.AbsoluteUri);
    Assert.Equal("https://staff.test/", staff.BaseAddress?.AbsoluteUri);
    Assert.Equal("https://scheduling.test/", scheduling.BaseAddress?.AbsoluteUri);
  }

  private HttpClient CreateClient()
  {
    return _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      BaseAddress = new Uri("https://localhost"),
      AllowAutoRedirect = false
    });
  }
}
