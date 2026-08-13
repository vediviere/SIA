using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class InitialPasswordException
    : ConflictException
{
  public InitialPasswordException()
      : base("La contraseña inicial de esta cuenta ya fue actualizada.")
  {
  }
}
