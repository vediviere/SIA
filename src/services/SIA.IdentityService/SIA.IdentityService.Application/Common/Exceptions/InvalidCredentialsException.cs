using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class InvalidCredentialsException
    : UnauthorizedException
{
  public InvalidCredentialsException()
      : base("Las credenciales proporcionadas no son válidas.")
  {
  }
}
