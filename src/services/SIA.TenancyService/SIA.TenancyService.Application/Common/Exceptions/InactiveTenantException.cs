using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.TenancyService.Application.Common.Exceptions;

public sealed class InactiveTenantException : ConflictException
{
  public InactiveTenantException(string instituteCode) : base($"La institución {instituteCode} no se encuentra activa.")
  {
  }
}
