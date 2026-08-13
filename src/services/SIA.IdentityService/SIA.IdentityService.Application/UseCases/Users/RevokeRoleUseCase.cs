using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class RevokeRoleUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly IRoleDataStore _roleDataStore;

  public RevokeRoleUseCase(IUserDataStore userDataStore, IRoleDataStore roleDataStore)
  {
    _userDataStore = userDataStore;
    _roleDataStore = roleDataStore;
  }

  public async Task ExecuteAsync(Guid userId, string roleCode, Guid tenantId, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
    }

    if (string.IsNullOrWhiteSpace(roleCode))
    {
      throw new ArgumentException("El rol es obligatorio.", nameof(roleCode));
    }

    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("La institución es obligatoria.", nameof(tenantId));
    }

    if (administratorUserId == Guid.Empty)
    {
      throw new ArgumentException("El administrador es obligatorio.", nameof(administratorUserId));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    var user = await _userDataStore.GetUserByIdAsync(userId, cancellationToken);

    if (user is null || user.TenantId != tenantId)
    {
      throw new UserNotFoundException();
    }

    var normalizedRoleCode = roleCode.Trim();
    var role = await _roleDataStore.GetRoleByCodeAsync(normalizedRoleCode, cancellationToken);

    if (role is null)
    {
      throw new RoleNotFoundException(normalizedRoleCode);
    }

    var userRole = await _userDataStore.GetActiveUserRoleAsync(user.Id, role.Id, cancellationToken);

    if (userRole is null)
    {
      throw new RoleRevocationException();
    }

    userRole.Revoke();

    var roleRevokedEvent = new UserRoleRevokedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = userRole.RevokedAtUtc!.Value,
      TenantId = user.TenantId,
      UserId = user.Id,
      RoleId = role.Id,
      RoleCode = role.Code,
      Version = 1
    };

    await _userDataStore.RevokeRoleAsync(user, userRole, roleRevokedEvent, administratorUserId, cancellationToken);
  }
}
