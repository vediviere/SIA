using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class ChangePasswordUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly IPasswordHasher _passwordHasher;

  public ChangePasswordUseCase(IUserDataStore userDataStore, IPasswordHasher passwordHasher)
  {
    _userDataStore = userDataStore;
    _passwordHasher = passwordHasher;
  }

  public async Task ExecuteAsync(Guid userId, ChangePasswordRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    if (userId == Guid.Empty) throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
    if (string.IsNullOrWhiteSpace(request.CurrentPassword)) throw new ArgumentException("La contraseña actual es obligatoria.", nameof(request.CurrentPassword));
    if (string.IsNullOrWhiteSpace(request.NewPassword)) throw new ArgumentException("La nueva contraseña es obligatoria.", nameof(request.NewPassword));
    if (correlationId == Guid.Empty) throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));

    var user = await _userDataStore.GetUserByIdAsync(userId, cancellationToken);

    if (user is null || user.Status != UserStatus.Active || !_passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
      throw new InvalidCredentialsException();

    var passwordHash = _passwordHasher.Hash(request.NewPassword);
    user.ChangePassword(passwordHash);

    var passwordChangedEvent = new PasswordChangedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = user.UpdatedAtUtc!.Value,
      TenantId = user.TenantId,
      UserId = user.Id,
      Version = 1
    };

    await _userDataStore.UpdatePasswordAsync(user, passwordChangedEvent, "PasswordChanged", cancellationToken);
  }
}
