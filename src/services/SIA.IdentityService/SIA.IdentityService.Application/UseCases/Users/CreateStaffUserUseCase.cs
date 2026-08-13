using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Contracts.Responses.Users;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class CreateStaffUserUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly IRoleDataStore _roleDataStore;
  private readonly IPasswordHasher _passwordHasher;

  public CreateStaffUserUseCase(IUserDataStore userDataStore, IRoleDataStore roleDataStore, IPasswordHasher passwordHasher)
  {
    _userDataStore = userDataStore;
    _roleDataStore = roleDataStore;
    _passwordHasher = passwordHasher;
  }

  public async Task<CreateStaffUserResponse> ExecuteAsync(CreateStaffUserRequest request, Guid tenantId, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken)
  {
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

    if (string.IsNullOrWhiteSpace(request.Email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(request.Email));
    }

    if (string.IsNullOrWhiteSpace(request.TemporaryPassword))
    {
      throw new ArgumentException("La contraseña provisional es obligatoria.", nameof(request.TemporaryPassword));
    }

    if (string.IsNullOrWhiteSpace(request.RoleCode))
    {
      throw new ArgumentException("El rol es obligatorio.", nameof(request.RoleCode));
    }

    var normalizedEmail = request.Email.Trim().ToLowerInvariant();

    var roleCode = request.RoleCode.Trim();

    if (string.Equals(roleCode, "Student", StringComparison.OrdinalIgnoreCase))
    {
      throw new InvalidStaffRoleException(roleCode);
    }

    var emailExists = await _userDataStore.EmailExistsAsync(normalizedEmail, cancellationToken);

    if (emailExists)
    {
      throw new UserEmailAlreadyExistsException(normalizedEmail);
    }

    var role = await _roleDataStore.GetRoleByCodeAsync(roleCode, cancellationToken);

    if (role is null)
    {
      throw new RoleNotFoundException(roleCode);
    }

    var passwordHash = _passwordHasher.Hash(request.TemporaryPassword);

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

    await _userDataStore.AddUserWithRoleAsync(user, userRole, userCreatedEvent, roleAssignedEvent, "StaffUserCreated", administratorUserId, cancellationToken);

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
