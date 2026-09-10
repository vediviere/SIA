using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SIA.AcademicService.Tests.Integration;

public class AcademicContextAuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _testKeyBase64;

    public AcademicContextAuthenticationTests(WebApplicationFactory<Program> factory)
    {
        var plainTextKey = "llave-de-prueba-tests-1234567890";
        _testKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainTextKey));

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Token:Issuer", "test-issuer"},
                    {"Token:Audience", "test-audience"},
                    {"Token:SigningKey", _testKeyBase64}
                });
            });
        });
    }

    [Fact]
    public async Task Get_WithoutToken_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/academic-context/educational-programs/{Guid.NewGuid()}");
        request.Headers.Add("tenantid", Guid.NewGuid().ToString());

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithValidToken_ShouldNotReturnUnauthorized()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/academic-context/educational-programs/{Guid.NewGuid()}");
        request.Headers.Add("tenantid", Guid.NewGuid().ToString());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GenerateValidToken());

        var response = await client.SendAsync(request);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private string GenerateValidToken()
    {
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_testKeyBase64));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, "test-user") };

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}