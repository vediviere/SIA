using SIA.IdentityService.Application.Models;

namespace SIA.IdentityService.Application.Interfaces.Security;

public interface IRefreshTokenService
{
  RefreshTokenResult Generate();
  string Hash(string token);
}
