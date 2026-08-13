using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Contracts.Responses.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class RefreshUseCase
{
  private readonly IRefreshTokenDataStore _refreshTokenDataStore;
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly IUserDataStore _userDataStore;
  private readonly IRoleDataStore _roleDataStore;
  private readonly ITokenGenerator _tokenGenerator;
  private readonly IPermissionDataStore _permissionDataStore;

  public RefreshUseCase(IRefreshTokenDataStore refreshTokenDataStore, IRefreshTokenService refreshTokenService, IUserDataStore userDataStore, IRoleDataStore roleDataStore, ITokenGenerator tokenGenerator, IPermissionDataStore permissionDataStore)
  {
    _refreshTokenDataStore = refreshTokenDataStore;
    _refreshTokenService = refreshTokenService;
    _userDataStore = userDataStore;
    _roleDataStore = roleDataStore;
    _tokenGenerator = tokenGenerator;
    _permissionDataStore = permissionDataStore;
  }

  public async Task<LoginResponse> ExecuteAsync(RefreshRequest request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      throw new ArgumentException("El refresh token es obligatorio.", nameof(request.RefreshToken));
    }

    var tokenHash = _refreshTokenService.Hash(request.RefreshToken);
    var currentToken = await _refreshTokenDataStore.GetByHashAsync(tokenHash, cancellationToken);

    if (currentToken is null || !currentToken.IsActive(DateTime.UtcNow))
    {
      throw new RefreshTokenException();
    }

    var user = await _userDataStore.GetUserByIdAsync(currentToken.UserId, cancellationToken);

    if (user is null || user.Status != UserStatus.Active || user.MustChangePassword)
    {
      throw new RefreshTokenException();
    }

    var roles = await _roleDataStore.GetActiveRolesAsync(user.Id, cancellationToken);
    var roleCodes = roles.Select(role => role.Code).ToArray();

    var permissions = await _permissionDataStore.GetActivePermissionsAsync(user.Id, cancellationToken);
    var permissionCodes = permissions.Select(permission => permission.Code).ToArray();

    var accessToken = _tokenGenerator.Generate(user, roleCodes, permissionCodes);
    var newRefreshResult = _refreshTokenService.Generate();
    var newRefreshToken = new RefreshToken(user.Id, newRefreshResult.TokenHash, newRefreshResult.ExpiresAtUtc);

    currentToken.Revoke();

    await _refreshTokenDataStore.RotateAsync(currentToken, newRefreshToken, cancellationToken);

    return new LoginResponse
    {
      UserId = user.Id,
      TenantId = user.TenantId,
      Email = user.Email,
      AccessToken = accessToken.Token,
      AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
      RefreshToken = newRefreshResult.Token,
      RefreshTokenExpiresAtUtc = newRefreshResult.ExpiresAtUtc,
      Roles = roleCodes
    };
  }
}
