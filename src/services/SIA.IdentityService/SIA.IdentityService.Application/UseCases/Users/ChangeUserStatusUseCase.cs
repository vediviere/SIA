using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class ChangeUserStatusUseCase
{
  private readonly IUserDataStore _userDataStore;

  public ChangeUserStatusUseCase(IUserDataStore userDataStore)
  {
    _userDataStore = userDataStore;
  }

  public async Task ExecuteAsync(Guid userId, UserStatus status, Guid tenantId, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
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

    if (user.Status == status)
    {
      return;
    }

    var previousStatus = user.Status;

    switch (status)
    {
      case UserStatus.Active:
        user.Activate();
        break;

      case UserStatus.Locked:
        user.Lock();
        break;

      case UserStatus.Inactive:
        user.Deactivate();
        break;

      default:
        throw new ArgumentOutOfRangeException(nameof(status));
    }

    await _userDataStore.ChangeStatusAsync(user, previousStatus, administratorUserId, correlationId, cancellationToken);
  }
}
