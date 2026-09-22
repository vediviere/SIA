using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SIA.WorkflowService.Api.Extensions;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(value, out var userId) || userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "El token no contiene un identificador de usuario válido.");
        }

        return userId;
    }

}
