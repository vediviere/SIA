namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class InvalidStaffRoleException : ArgumentException
{
  public InvalidStaffRoleException(string roleCode) : base($"El rol {roleCode} no puede asignarse mediante la gestión de personal.")
  {
  }
}
