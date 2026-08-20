namespace SIA.TenancyService.Application.Common.Exceptions;

public sealed class InvalidTenantEmailException : ArgumentException
{
  public InvalidTenantEmailException(string instituteCode) : base($"El correo no pertenece a la institución {instituteCode}.")
  {
  }
}
