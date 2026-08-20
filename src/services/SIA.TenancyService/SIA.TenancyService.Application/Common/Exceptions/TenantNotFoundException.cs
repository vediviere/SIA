using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.TenancyService.Application.Common.Exceptions;

public sealed class TenantNotFoundException : NotFoundException
{
  public TenantNotFoundException(string instituteCode) : base($"No se encontró una institución con el código {instituteCode}.")
  {
  }
}
