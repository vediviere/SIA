using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class AssignRoleUseCase
{
  private readonly IUserDataStore _userDataStore;
  private readonly IRoleDataStore _roleDataStore;

  public AssignRoleUseCase(IUserDataStore userDataStore, IRoleDataStore roleDataStore)
  {
    _userDataStore = userDataStore;
    _roleDataStore = roleDataStore;
  }

  public async Task ExecuteAsync(Guid userId, AssignRoleRequest request, Guid tenantId, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
    }

    if (string.IsNullOrWhiteSpace(request.RoleCode))
    {
      throw new ArgumentException("El rol es obligatorio.", nameof(request.RoleCode));
    }

    var user = await _userDataStore.GetUserByIdAsync(userId, cancellationToken);

    if (user is null || user.TenantId != tenantId)
    {
      throw new UserNotFoundException();
    }

    var roleCode = request.RoleCode.Trim();

    if (roleCode.Equals("Student", StringComparison.OrdinalIgnoreCase))
    {
      throw new InvalidStaffRoleException(roleCode);
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

    var role = await _roleDataStore.GetRoleByCodeAsync(roleCode, cancellationToken);

    if (role is null)
    {
      throw new RoleNotFoundException(roleCode);
    }

    if (await _userDataStore.HasActiveRoleAsync(user.Id, role.Id, cancellationToken))
    {
      throw new RoleAssignmentException();
    }

    var userRole = new UserRole(user.Id, role.Id);

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

    await _userDataStore.AddRoleAsync(user, userRole, roleAssignedEvent, administratorUserId, cancellationToken);
  }
}
