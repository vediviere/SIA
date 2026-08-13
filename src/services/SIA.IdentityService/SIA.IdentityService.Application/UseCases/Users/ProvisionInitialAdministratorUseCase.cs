using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;

using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Contracts.Responses.Users;

using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class ProvisionInitialAdministratorUseCase
{
  private const string AdministratorRoleCode = "Administrator";

  private readonly IUserDataStore _userDataStore;
  private readonly IRoleDataStore _roleDataStore;
  private readonly IPasswordHasher _passwordHasher;

  public ProvisionInitialAdministratorUseCase(IUserDataStore userDataStore, IRoleDataStore roleDataStore, IPasswordHasher passwordHasher)
  {
    _userDataStore = userDataStore;
    _roleDataStore = roleDataStore;
    _passwordHasher = passwordHasher;
  }

  public async Task<CreateStaffUserResponse> ExecuteAsync(Guid tenantId, string email, string temporaryPassword, Guid correlationId, CancellationToken cancellationToken)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("La institución es obligatoria.", nameof(tenantId));
    }

    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(email));
    }

    if (string.IsNullOrWhiteSpace(temporaryPassword))
    {
      throw new ArgumentException("La contraseña provisional es obligatoria.", nameof(temporaryPassword));
    }

    if (correlationId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de correlación es obligatorio.", nameof(correlationId));
    }

    var normalizedEmail = email.Trim().ToLowerInvariant();

    var role = await _roleDataStore.GetRoleByCodeAsync(AdministratorRoleCode, cancellationToken);

    if (role is null)
    {
      throw new RoleNotFoundException(AdministratorRoleCode);
    }

    var administratorAlreadyProvisioned = await _userDataStore.HasRoleAssignmentInTenantAsync(tenantId, role.Id, cancellationToken);

    if (administratorAlreadyProvisioned)
    {
      throw new InitialAdministratorException(tenantId);
    }

    var emailExists = await _userDataStore.EmailExistsAsync(normalizedEmail, cancellationToken);

    if (emailExists)
    {
      throw new UserEmailAlreadyExistsException(normalizedEmail);
    }

    var passwordHash = _passwordHasher.Hash(temporaryPassword);

    var user = new User(tenantId, normalizedEmail, passwordHash, mustChangePassword: true);

    var userRole = new UserRole(user.Id, role.Id);

    var userCreatedEvent = new UserCreatedIntegrationEvent
        {
          EventId = Guid.NewGuid(),
          CorrelationId = correlationId,
          OccurredAtUtc = user.CreatedAtUtc,
          TenantId = user.TenantId,
          UserId = user.Id,
          Version = 1
        };

    var roleAssignedEvent = new UserRoleAssignedIntegrationEvent
        {
          EventId = Guid.NewGuid(),
          CorrelationId = correlationId,
          OccurredAtUtc = userRole.CreatedAtUtc,
          TenantId = user.TenantId,
          UserId = user.Id,
          RoleId = role.Id,
          RoleCode = role.Code,
          Version = 1
        };

    await _userDataStore.AddUserWithRoleAsync(user, userRole, userCreatedEvent, roleAssignedEvent, "InitialAdministratorProvisioned", actorUserId: null, cancellationToken);

    return new CreateStaffUserResponse
    {
      Id = user.Id,
      TenantId = user.TenantId,
      Email = user.Email,
      RoleId = role.Id,
      RoleCode = role.Code,
      MustChangePassword = user.MustChangePassword,
      CreatedAtUtc = user.CreatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
