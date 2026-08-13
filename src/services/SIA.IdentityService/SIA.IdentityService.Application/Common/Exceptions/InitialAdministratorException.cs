using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class InitialAdministratorException : ConflictException
{
  public InitialAdministratorException(Guid tenantId) : base($"La institución {tenantId} ya cuenta con un administrador inicial provisionado.")
  {
  }
}
