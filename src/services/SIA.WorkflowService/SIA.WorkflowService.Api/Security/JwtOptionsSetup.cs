using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace SIA.WorkflowService.Api.Security;

public sealed class JwtOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        var signingKey = _configuration["Token:SigningKey"]
            ?? throw new InvalidOperationException("Token:SigningKey no está configurado.");

        var issuer = _configuration["Token:Issuer"]
            ?? throw new InvalidOperationException("Token:Issuer no está configurado.");

        var audience = _configuration["Token:Audience"]
            ?? throw new InvalidOperationException("Token:Audience no está configurado.");

        var signingKeyBytes = Convert.FromBase64String(signingKey);

        if (signingKeyBytes.Length < 32)
        {
            throw new InvalidOperationException("Token:SigningKey debe contener al menos 256 bits.");
        }

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            NameClaimType = "email",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var userIdValue = context.Principal?
                    .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!Guid.TryParse(userIdValue, out var userId) ||
                    userId == Guid.Empty)
                {
                    context.Fail(
                        "La identidad no contiene un identificador de usuario válido.");

                    return Task.CompletedTask;
                }

                var tenantIdValue = context.Principal?
                    .FindFirst("tenant_id")?.Value;

                if (!Guid.TryParse(tenantIdValue, out var tenantId) ||
                    tenantId == Guid.Empty)
                {
                    context.Fail(
                        "La identidad no contiene un TenantId válido.");

                    return Task.CompletedTask;
                }

                return Task.CompletedTask;
            }
        };
    }
}