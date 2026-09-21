using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SIA.WorkflowService.IntegrationTests.Infrastructure;

public static class JwtTokenFactory
{
    public static string Create(
        Guid userId,
        Guid tenantId,
        string role)
    {
        var signingKeyBytes =
            Convert.FromBase64String(WorkflowApiFactory.TestSigningKey);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(signingKeyBytes),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("role", role)
        };

        var token = new JwtSecurityToken(
            issuer: "SIA.IdentityService",
            audience: "SIA.Platform",
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}