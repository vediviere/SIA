using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SIA.TeacherBff.Infrastructure.Errors;
using SIA.TeacherBff.Infrastructure.Http;

namespace SIA.TeacherBff.Infrastructure.Authentication;

public static class AuthExtensions
{
  public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
  {
    var issuer = configuration["Token:Issuer"] ?? throw new InvalidOperationException("Token:Issuer no está configurado.");
    var audience = configuration["Token:Audience"] ?? throw new InvalidOperationException("Token:Audience no está configurado.");
    var signingKey = configuration["Token:SigningKey"] ?? throw new InvalidOperationException("Token:SigningKey no está configurado.");
    var signingKeyBytes = Convert.FromBase64String(signingKey);

    if (signingKeyBytes.Length < 32)
    {
      throw new InvalidOperationException("Token:SigningKey debe contener al menos 256 bits.");
    }

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
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
          var value = context.Principal?.FindFirst("tenant_id")?.Value;

          if (!Guid.TryParse(value, out var tenantId) || tenantId == Guid.Empty)
          {
            context.Fail("La identidad no contiene un TenantId válido.");
          }

          return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
          context.HandleResponse();
          await WriteErrorAsync(context.HttpContext, StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "La sesión no está autenticada.");
        },
        OnForbidden = async context =>
        {
          await WriteErrorAsync(context.HttpContext, StatusCodes.Status403Forbidden, "FORBIDDEN", "No cuenta con permisos para realizar la operación.");
        }
      };
    });

    services.AddAuthorization();
    return services;
  }

  private static async Task WriteErrorAsync(HttpContext context, int status, string code, string message)
  {
    var correlationId = context.Items[CorrelationConstants.ItemKey] is Guid value ? value : Guid.NewGuid();

    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new ErrorResponse
    {
      Code = code,
      Message = message,
      CorrelationId = correlationId
    });
  }
}
