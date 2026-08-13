using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Contracts.Responses.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class LoginUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly IRoleDataStore _roleDataStore;
  private readonly IPasswordHasher _passwordHasher;
  private readonly ITokenGenerator _tokenGenerator;
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly IRefreshTokenDataStore _refreshTokenDataStore;
  private readonly IPermissionDataStore _permissionDataStore;

  public LoginUseCase(IUserDataStore userDataStore, IRoleDataStore roleDataStore, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator, IRefreshTokenService refreshTokenService, IRefreshTokenDataStore refreshTokenDataStore, IPermissionDataStore permissionDataStore)
  {
    _userDataStore = userDataStore;
    _roleDataStore = roleDataStore;
    _passwordHasher = passwordHasher;
    _tokenGenerator = tokenGenerator;
    _refreshTokenService = refreshTokenService;
    _refreshTokenDataStore = refreshTokenDataStore;
    _permissionDataStore = permissionDataStore;
  }

  public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(request.Email));
    }

    if (string.IsNullOrWhiteSpace(request.Password))
    {
      throw new ArgumentException("La contraseña es obligatoria.", nameof(request.Password));
    }

    var user = await _userDataStore.GetUserByEmailAsync(request.Email, cancellationToken);

    if (user is null || user.Status != UserStatus.Active || !_passwordHasher.Verify(user.PasswordHash, request.Password))
    {
      throw new InvalidCredentialsException();
    }

    if (user.MustChangePassword)
    {
      throw new PasswordChangeException();
    }

    var roles = await _roleDataStore.GetActiveRolesAsync(user.Id, cancellationToken);

    var roleCodes = roles.Select(role => role.Code).ToArray();

    var permissions = await _permissionDataStore.GetActivePermissionsAsync(user.Id, cancellationToken);
    var permissionCodes = permissions.Select(permission => permission.Code).ToArray();

    var token = _tokenGenerator.Generate(user, roleCodes, permissionCodes);

    var refreshTokenResult = _refreshTokenService.Generate();

    var refreshToken = new RefreshToken(user.Id, refreshTokenResult.TokenHash, refreshTokenResult.ExpiresAtUtc);

    await _refreshTokenDataStore.AddAsync(refreshToken, cancellationToken);

    return new LoginResponse
    {
      UserId = user.Id,
      TenantId = user.TenantId,
      Email = user.Email,
      AccessToken = token.Token,
      AccessTokenExpiresAtUtc = token.ExpiresAtUtc,
      RefreshToken = refreshTokenResult.Token,
      RefreshTokenExpiresAtUtc = refreshTokenResult.ExpiresAtUtc,
      Roles = roleCodes
    };
  }
}
