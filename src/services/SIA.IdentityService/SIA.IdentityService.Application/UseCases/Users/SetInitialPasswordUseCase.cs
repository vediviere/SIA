using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class SetInitialPasswordUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly IPasswordHasher _passwordHasher;

  public SetInitialPasswordUseCase(IUserDataStore userDataStore, IPasswordHasher passwordHasher)
  {
    _userDataStore = userDataStore;
    _passwordHasher = passwordHasher;
  }

  public async Task ExecuteAsync(SetInitialPasswordRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(request.Email));
    }

    if (string.IsNullOrWhiteSpace(request.TemporaryPassword))
    {
      throw new ArgumentException("La contraseña provisional es obligatoria.", nameof(request.TemporaryPassword));
    }

    if (string.IsNullOrWhiteSpace(request.NewPassword))
    {
      throw new ArgumentException("La nueva contraseña es obligatoria.", nameof(request.NewPassword));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    var user = await _userDataStore.GetUserByEmailAsync(request.Email, cancellationToken);

    if (user is null || user.Status != UserStatus.Active || !_passwordHasher.Verify(user.PasswordHash, request.TemporaryPassword))
    {
      throw new InvalidCredentialsException();
    }

    if (!user.MustChangePassword)
    {
      throw new InitialPasswordException();
    }

    var newPasswordHash = _passwordHasher.Hash(request.NewPassword);

    user.ChangePassword(newPasswordHash);

    var passwordChangedEvent = new PasswordChangedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = user.UpdatedAtUtc!.Value,
      TenantId = user.TenantId,
      UserId = user.Id,
      Version = 1
    };

    await _userDataStore.UpdatePasswordAsync(user, passwordChangedEvent, "InitialPasswordChanged", cancellationToken);
  }
}
