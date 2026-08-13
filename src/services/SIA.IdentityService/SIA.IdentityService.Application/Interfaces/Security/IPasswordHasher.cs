namespace SIA.IdentityService.Application.Interfaces.Security;

public interface IPasswordHasher
{
  string Hash(string password);

  bool Verify(string passwordHash, string password);
}
