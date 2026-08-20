using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Interfaces.Tenancy;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Contracts.Responses.Users;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class SelfRegisterUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly ITenantResolver _tenantResolver;
  private readonly IPasswordHasher _passwordHasher;

  public SelfRegisterUseCase(IUserDataStore userDataStore, ITenantResolver tenantResolver, IPasswordHasher passwordHasher)
  {
    _userDataStore = userDataStore;
    _tenantResolver = tenantResolver;
    _passwordHasher = passwordHasher;
  }

  public async Task<SelfRegisterResponse> ExecuteAsync(SelfRegisterRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.InstituteCode))
    {
      throw new ArgumentException("El código institucional es obligatorio.", nameof(request.InstituteCode));
    }

    if (string.IsNullOrWhiteSpace(request.Email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(request.Email));
    }

    if (string.IsNullOrWhiteSpace(request.Password))
    {
      throw new ArgumentException("La contraseña es obligatoria.", nameof(request.Password));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    var instituteCode = request.InstituteCode.Trim().ToUpperInvariant();
    var normalizedEmail = request.Email.Trim().ToLowerInvariant();

    var tenantId = await _tenantResolver.ResolveTenantIdAsync(instituteCode, normalizedEmail, cancellationToken);

    if (tenantId is null || tenantId == Guid.Empty)
    {
      throw new InstitutionNotFoundException(instituteCode);
    }

    var emailExists = await _userDataStore.EmailExistsAsync(normalizedEmail, cancellationToken);

    if (emailExists)
    {
      throw new UserEmailAlreadyExistsException(normalizedEmail);
    }

    var passwordHash = _passwordHasher.Hash(request.Password);
    var user = new User(tenantId.Value, normalizedEmail, passwordHash, mustChangePassword: false);

    var userCreatedEvent = new UserCreatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = user.CreatedAtUtc,
      TenantId = user.TenantId,
      UserId = user.Id,
      Version = 1
    };

    await _userDataStore.AddUserAsync(user, userCreatedEvent, "UserSelfRegistered", cancellationToken);

    return new SelfRegisterResponse
    {
      Id = user.Id,
      TenantId = user.TenantId,
      Email = user.Email,
      MustChangePassword = user.MustChangePassword,
      CreatedAtUtc = user.CreatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
