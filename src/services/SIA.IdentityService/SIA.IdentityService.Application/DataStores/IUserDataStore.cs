using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Application.Interfaces.DataStores;

public interface IUserDataStore
{
  Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
  Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
  Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
  Task<bool> HasRoleAssignmentInTenantAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken);
  Task<bool> HasActiveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
  Task<UserRole?> GetActiveUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
  Task AddUserAsync(User user, UserCreatedIntegrationEvent userCreatedEvent, string auditAction, CancellationToken cancellationToken);

  Task AddUserWithRoleAsync(User user, UserRole userRole, UserCreatedIntegrationEvent userCreatedEvent, UserRoleAssignedIntegrationEvent roleAssignedEvent, string userAuditAction, Guid? actorUserId, CancellationToken cancellationToken);
  Task UpdatePasswordAsync(User user, PasswordChangedIntegrationEvent passwordChangedEvent, string auditAction, CancellationToken cancellationToken);
  Task AddRoleAsync(User user, UserRole userRole, UserRoleAssignedIntegrationEvent roleAssignedEvent, Guid administratorUserId, CancellationToken cancellationToken);
  Task RevokeRoleAsync(User user, UserRole userRole, UserRoleRevokedIntegrationEvent roleRevokedEvent, Guid administratorUserId, CancellationToken cancellationToken);
  Task ChangeStatusAsync(User user, UserStatus previousStatus, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken);
}
