using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class UserNotFoundException : NotFoundException
{
  public UserNotFoundException() : base("No se encontró el usuario solicitado.") { }
}
