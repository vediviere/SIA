using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.IdentityService.Application.Common.Exceptions;

public sealed class UserEmailAlreadyExistsException : ConflictException
{
  public UserEmailAlreadyExistsException(string email) : base($"Ya existe una cuenta registrada con el correo {email}.")
  {
  }
}
