using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Tests.Common.Fakes;

public sealed class FakeUserDataStore : IUserDataStore
{
  public User? UserById { get; set; }
  public User? UserByEmail { get; set; }
  public UserRole? ActiveUserRole { get; set; }

  public bool EmailExistsResult { get; set; }
  public bool HasRoleAssignmentResult { get; set; }
  public bool HasActiveRoleResult { get; set; }


  public bool UserWithRoleAdded { get; private set; }
  public bool PasswordUpdated { get; private set; }
  public bool RoleAdded { get; private set; }
  public bool RoleRevoked { get; private set; }

  public string? UserAuditAction { get; private set; }
  public Guid? ActorUserId { get; private set; }

  public bool StatusChanged { get; private set; }
  public Task ChangeStatusAsync(User user, UserStatus previousStatus, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken)
  {
    StatusChanged = true;
    return Task.CompletedTask;
  }

  public FakeUserDataStore() { }

  public FakeUserDataStore(User? user)
  {
    UserById = user;
    UserByEmail = user;
  }

  public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken) => Task.FromResult(EmailExistsResult);

  public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken) => Task.FromResult(UserById);

  public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken) => Task.FromResult(UserByEmail);

  public Task<bool> HasRoleAssignmentInTenantAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken) => Task.FromResult(HasRoleAssignmentResult);

  public Task<bool> HasActiveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken) => Task.FromResult(HasActiveRoleResult);

  public Task<UserRole?> GetActiveUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken) => Task.FromResult(ActiveUserRole);

  public Task AddUserWithRoleAsync(User user, UserRole userRole, UserCreatedIntegrationEvent userCreatedEvent, UserRoleAssignedIntegrationEvent roleAssignedEvent, string userAuditAction, Guid? actorUserId, CancellationToken cancellationToken)
  {
    UserWithRoleAdded = true;
    UserAuditAction = userAuditAction;
    ActorUserId = actorUserId;
    return Task.CompletedTask;
  }

  public Task UpdatePasswordAsync(User user, PasswordChangedIntegrationEvent passwordChangedEvent, string auditAction, CancellationToken cancellationToken)
  {
    PasswordUpdated = true;
    return Task.CompletedTask;
  }

  public Task AddRoleAsync(User user, UserRole userRole, UserRoleAssignedIntegrationEvent roleAssignedEvent, Guid administratorUserId, CancellationToken cancellationToken)
  {
    RoleAdded = true;
    return Task.CompletedTask;
  }

  public Task RevokeRoleAsync(User user, UserRole userRole, UserRoleRevokedIntegrationEvent roleRevokedEvent, Guid administratorUserId, CancellationToken cancellationToken)
  {
    RoleRevoked = true;
    return Task.CompletedTask;
  }
}
