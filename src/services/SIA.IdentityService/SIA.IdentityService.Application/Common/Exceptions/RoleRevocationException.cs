using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class RoleRevocationException : ConflictException
{
  public RoleRevocationException() : base("El usuario no tiene asignado este rol.") { }
}
