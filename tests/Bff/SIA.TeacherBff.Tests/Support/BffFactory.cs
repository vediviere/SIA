using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIA.TeacherBff.Tests.Support;

public sealed class BffFactory : WebApplicationFactory<Program>
{
  private const string Issuer = "teacher-bff-tests";
  private const string Audience = "sia-tests";
  private const string PlainKey = "teacher-bff-tests-signing-key-with-more-than-32-bytes";
  private readonly string _signingKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(PlainKey));

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");
    builder.UseSetting("Token:Issuer", Issuer);
    builder.UseSetting("Token:Audience", Audience);
    builder.UseSetting("Token:SigningKey", _signingKey);
    builder.UseSetting("Services:AcademicService:BaseUrl", "https://academic.test/");
    builder.UseSetting("Services:AcademicStaffService:BaseUrl", "https://staff.test/");
    builder.UseSetting("Services:SchedulingService:BaseUrl", "https://scheduling.test/");

    builder.ConfigureServices(services =>
    {
      services.AddControllers().AddApplicationPart(typeof(ProbeController).Assembly);
    });
  }

  public string CreateToken(Guid tenantId)
  {
    return CreateToken(
    [
      new Claim(JwtRegisteredClaimNames.Sub, "teacher-test-user"),
      new Claim("tenant_id", tenantId.ToString())
    ]);
  }

  public string CreateTokenWithoutTenant()
  {
    return CreateToken(
    [
      new Claim(JwtRegisteredClaimNames.Sub, "teacher-test-user")
    ]);
  }

  private string CreateToken(IEnumerable<Claim> claims)
  {
    var key = new SymmetricSecurityKey(Convert.FromBase64String(_signingKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(Issuer, Audience, claims, expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
