using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace SIA.PublicGateway.Tests;

public sealed class RateLimiterTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RateLimiterTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "RateLimiting:Global:PermitLimit", "100" },
                    { "RateLimiting:IdentityAuth:PermitLimit", "5" },
                    { "ReverseProxy:Routes:identity-users:ClusterId", "test-cluster" },
                    { "ReverseProxy:Routes:identity-users:Match:Path", "/api/users/{**catch-all}" },
                    { "ReverseProxy:Routes:identity-users:RateLimiterPolicy", "IdentityAuthPolicy" },
                    { "ReverseProxy:Clusters:test-cluster:Destinations:test-dest:Address", "http://localhost" }
                });
            });
        });
    }

    [Fact]
    public async Task PostLogin_WhenLimitExceeded_ShouldReturnTooManyRequests()
    {
        var client = _factory.CreateClient();
        var url = "/api/users/login";

        for (int i = 0; i < 5; i++)
        {
            var response = await client.PostAsync(url, null);

            Assert.NotEqual(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        var blockedResponse = await client.PostAsync(url, null);

        Assert.Equal(HttpStatusCode.TooManyRequests, blockedResponse.StatusCode);
    }

    [Fact]
    public async Task GetHealth_WithMultipleRequests_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var url = "/health";

        for (int i = 0; i < 105; i++)
        {
            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}