using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class RoleAssignmentException : ConflictException
{
  public RoleAssignmentException() : base("El usuario ya tiene asignado este rol.") { }
}
