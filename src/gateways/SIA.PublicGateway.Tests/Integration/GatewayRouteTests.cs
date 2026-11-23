using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SIA.PublicGateway.Tests.Integration;

public sealed class GatewayRouteTests : IAsyncLifetime
{
    private const string Issuer = "gateway-tests";
    private const string Audience = "sia-tests";
    private const string PlainSigningKey = "gateway-tests-signing-key-with-more-than-32-bytes";
    private readonly string _signingKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(PlainSigningKey));
    private WebApplication? _adminBff;
    private WebApplication? _teacherBff;
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;
    private int _forwardedRequests;
    private string? _forwardedAuthorization;
    private string? _forwardedCorrelationId;

    private int _teacherForwardedRequests;
    private string? _teacherForwardedAuthorization;
    private string? _teacherForwardedCorrelationId;
    private string? _teacherForwardedPath;


    public async Task InitializeAsync()
    {
        var adminBuilder = WebApplication.CreateBuilder();
        adminBuilder.WebHost.UseUrls("http://127.0.0.1:0");
        _adminBff = adminBuilder.Build();

        var teacherBuilder = WebApplication.CreateBuilder();
        teacherBuilder.WebHost.UseUrls("http://127.0.0.1:0");
        _teacherBff = teacherBuilder.Build();



        _adminBff.MapGet("/api/academic-planning/context/{educationalProgramId:guid}", async context =>
        {
            Interlocked.Increment(ref _forwardedRequests);
            _forwardedAuthorization = context.Request.Headers.Authorization.ToString();
            _forwardedCorrelationId = context.Request.Headers["X-Correlation-Id"].ToString();
            context.Response.Headers["X-Backend"] = "AdminBff";
            await context.Response.WriteAsJsonAsync(new { source = "AdminBff" });
        });

        _adminBff.MapGet("/api/review-processes", async context =>
        {
            Interlocked.Increment(ref _forwardedRequests);
            _forwardedAuthorization = context.Request.Headers.Authorization.ToString();
            _forwardedCorrelationId = context.Request.Headers["X-Correlation-Id"].ToString();
            context.Response.Headers["X-Backend"] = "AdminBff";
            await context.Response.WriteAsJsonAsync(new { source = "AdminBff" });
        });

        await _adminBff.StartAsync();

        var server = _adminBff.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        var adminBffAddress = addresses?.Single() ?? throw new InvalidOperationException("No se pudo obtener la dirección del AdminBff simulado.");

        _teacherBff.MapGet("/{**catch-all}", async context =>
        {
            Interlocked.Increment(ref _teacherForwardedRequests);

            _teacherForwardedAuthorization = context.Request.Headers.Authorization.ToString();

            _teacherForwardedCorrelationId = context.Request.Headers["X-Correlation-Id"].ToString();

            _teacherForwardedPath = context.Request.Path.ToString();

            context.Response.Headers["X-Backend"] = "TeacherBff";

            await context.Response.WriteAsJsonAsync(new
            {
                source = "TeacherBff",
                path = context.Request.Path.ToString()
            });
        });

        await _teacherBff.StartAsync();

        var teacherServer = _teacherBff.Services.GetRequiredService<IServer>();
        var teacherAddresses = teacherServer.Features.Get<IServerAddressesFeature>()?.Addresses;

        var teacherBffAddress = teacherAddresses?.Single() ?? throw new InvalidOperationException(
                "No se pudo obtener la dirección del TeacherBff simulado."
        );

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("Token:Issuer", Issuer);
            builder.UseSetting("Token:Audience", Audience);
            builder.UseSetting("Token:SigningKey", _signingKey);

            builder.UseSetting("ReverseProxy:Routes:admin-bff-academic-planning:ClusterId", "admin-bff");
            builder.UseSetting("ReverseProxy:Routes:admin-bff-academic-planning:AuthorizationPolicy", "Authenticated");
            builder.UseSetting("ReverseProxy:Routes:admin-bff-academic-planning:Match:Path", "/api/academic-planning/{**catch-all}");

            builder.UseSetting("ReverseProxy:Routes:admin-bff-review-processes:ClusterId", "admin-bff");
            builder.UseSetting("ReverseProxy:Routes:admin-bff-review-processes:AuthorizationPolicy", "Authenticated");
            builder.UseSetting("ReverseProxy:Routes:admin-bff-review-processes:Match:Path", "/api/review-processes/{**catch-all}");

            builder.UseSetting("ReverseProxy:Clusters:admin-bff:Destinations:admin-bff-primary:Address", $"{adminBffAddress}/");

            builder.UseSetting("ReverseProxy:Routes:teacher-bff:ClusterId", "teacher-bff");
            builder.UseSetting("ReverseProxy:Routes:teacher-bff:AuthorizationPolicy", "Authenticated");
            builder.UseSetting("ReverseProxy:Routes:teacher-bff:Match:Path", "/api/teacher/{**catch-all}");
            builder.UseSetting("ReverseProxy:Clusters:teacher-bff:Destinations:teacher-bff-primary:Address", $"{teacherBffAddress}/");
            builder.UseSetting("ReverseProxy:Routes:teacher-bff:Transforms:0:PathRemovePrefix", "/api/teacher");

        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Health_WithoutToken_ReturnsOk()
    {
        var response = await Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AcademicPlanning_WithoutToken_ReturnsUnauthorizedWithoutForwarding()
    {
        var response = await Client.GetAsync($"/api/academic-planning/context/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, Volatile.Read(ref _forwardedRequests));
    }

    [Fact]
    public async Task AcademicPlanning_WithValidToken_ForwardsTokenAndCorrelationId()
    {
        var tenantId = Guid.NewGuid();
        var token = GenerateToken(tenantId);
        var correlationId = Guid.NewGuid().ToString();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/academic-planning/context/{Guid.NewGuid()}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Correlation-Id", correlationId);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, Volatile.Read(ref _forwardedRequests));
        Assert.Equal($"Bearer {token}", _forwardedAuthorization);
        Assert.Equal(correlationId, _forwardedCorrelationId);
        Assert.Equal("AdminBff", response.Headers.GetValues("X-Backend").Single());
        Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());
    }

    [Fact]
    public async Task ReviewProcesses_WithoutToken_ReturnsUnauthorizedWithoutForwarding()
    {
        var response = await Client.GetAsync("/api/review-processes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, Volatile.Read(ref _forwardedRequests));
    }

    [Fact]
    public async Task ReviewProcesses_WithValidToken_ForwardsTokenAndCorrelationId()
    {
        var tenantId = Guid.NewGuid();
        var token = GenerateToken(tenantId);
        var correlationId = Guid.NewGuid().ToString();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/review-processes");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Correlation-Id", correlationId);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, Volatile.Read(ref _forwardedRequests));
        Assert.Equal($"Bearer {token}", _forwardedAuthorization);
        Assert.Equal(correlationId, _forwardedCorrelationId);
        Assert.Equal("AdminBff", response.Headers.GetValues("X-Backend").Single());
        Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());
    }

    [Fact]
    public async Task TeacherBff_WithoutToken_ReturnsUnauthorizedWithoutForwarding()
    {
        var response = await Client.GetAsync("/api/teacher/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        Assert.Equal(0, Volatile.Read(ref _teacherForwardedRequests));
    }

    [Fact]
    public async Task TeacherBff_WithValidToken_ForwardsTokenAndCorrelationId()
    {
        var tenantId = Guid.NewGuid();
        var token = GenerateToken(tenantId);
        var correlationId = Guid.NewGuid().ToString();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/teacher/profile");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Headers.Add("X-Correlation-Id", correlationId);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(1, Volatile.Read(ref _teacherForwardedRequests));

        Assert.Equal($"Bearer {token}", _teacherForwardedAuthorization);

        Assert.Equal(correlationId, _teacherForwardedCorrelationId);

        Assert.Equal("TeacherBff", response.Headers.GetValues("X-Backend").Single());

        Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());

        Assert.Equal("/profile", _teacherForwardedPath);
    }

    [Fact]
    public async Task UnknownRoute_ReturnsNotFoundWithoutForwarding()
    {
        var response = await Client.GetAsync("/api/unknown/resource");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        Assert.Equal(0, Volatile.Read(ref _forwardedRequests));

        Assert.Equal(0, Volatile.Read(ref _teacherForwardedRequests));
    }


    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();

        if (_adminBff is not null)
        {
            await _adminBff.StopAsync();
            await _adminBff.DisposeAsync();
        }

        if (_teacherBff is not null)
        {
            await _teacherBff.StopAsync();
            await _teacherBff.DisposeAsync();
        }
    }

    private HttpClient Client => _client ??
        throw new InvalidOperationException("El cliente de prueba no fue inicializado.");

    private string GenerateToken(Guid tenantId)
    {
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_signingKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
      new Claim(JwtRegisteredClaimNames.Sub, "gateway-test-user"),
      new Claim("tenant_id", tenantId.ToString())
    };
        var token = new JwtSecurityToken(Issuer, Audience, claims, expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}