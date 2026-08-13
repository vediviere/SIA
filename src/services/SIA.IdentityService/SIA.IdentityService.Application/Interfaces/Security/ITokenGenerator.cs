using SIA.IdentityService.Application.Models;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.Interfaces.Security;

public interface ITokenGenerator
{
  TokenResult Generate(User user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions);
}
