using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class PasswordChangeException
    : ConflictException
{
  public PasswordChangeException()
      : base("Debes actualizar la contraseña provisional antes de iniciar sesión.")
  {
  }
}
