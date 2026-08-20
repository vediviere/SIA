using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class InstitutionNotFoundException : NotFoundException
{
  public InstitutionNotFoundException(string institutionCode) : base($"No existe una institución disponible para el código {institutionCode}.")
  {
  }
}
