using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class RoleNotFoundException : NotFoundException
{
  public RoleNotFoundException(string roleCode) : base($"No se encontró el rol {roleCode}.")
  {
  }
}
