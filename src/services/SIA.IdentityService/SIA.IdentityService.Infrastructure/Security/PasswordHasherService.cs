using Microsoft.AspNetCore.Identity;
using SIA.IdentityService.Application.Interfaces.Security;

namespace SIA.IdentityService.Infrastructure.Security;

public sealed class PasswordHasherService : IPasswordHasher
{
  private sealed class PasswordHashContext
  {
  }

  private static readonly PasswordHashContext Context = new();

  private readonly PasswordHasher<PasswordHashContext> _passwordHasher = new();

  public string Hash(string password)
  {
    if (string.IsNullOrWhiteSpace(password))
    {
      throw new ArgumentException("La contraseña es obligatoria.", nameof(password));
    }

    return _passwordHasher.HashPassword(Context, password);
  }

  public bool Verify(string passwordHash, string password)
  {
    if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(password))
    {
      return false;
    }

    var result = _passwordHasher.VerifyHashedPassword(Context, passwordHash, password);

    return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
  }
}
