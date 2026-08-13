using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class RefreshTokenException : UnauthorizedException
{
  public RefreshTokenException() : base("El refresh token no es válido o ya expiró.") { }
}
