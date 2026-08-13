using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;
using SIA.IdentityService.Infrastructure.Persistence.Contexts;
using SIA.IdentityService.Infrastructure.Persistence.Entities;

namespace SIA.IdentityService.Infrastructure.Persistence.DataStores;

public sealed class UserDataStore : IUserDataStore
{
  private readonly IdentityDbContext _dbContext;

  public UserDataStore(IdentityDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
  {
    var normalizedEmail = email.Trim().ToLowerInvariant();

    return _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
  }

  public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    return _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
  }

  public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
  {
    var normalizedEmail = email.Trim().ToLowerInvariant();

    return _dbContext.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
  }

  public async Task AddUserWithRoleAsync(User user, UserRole userRole, UserCreatedIntegrationEvent userCreatedEvent, UserRoleAssignedIntegrationEvent roleAssignedEvent, string userAuditAction, Guid? actorUserId, CancellationToken cancellationToken)
  {
    var userCreatedOutbox = new OutboxMessage(UserIntegrationEventTypes.UserCreatedV1, JsonSerializer.Serialize(userCreatedEvent), userCreatedEvent.CorrelationId);

    var roleAssignedOutbox = new OutboxMessage(UserIntegrationEventTypes.UserRoleAssignedV1, JsonSerializer.Serialize(roleAssignedEvent), roleAssignedEvent.CorrelationId);

    var userAuditLog = new AuditLog(user.TenantId, userAuditAction, "User", user.Id.ToString(), userCreatedEvent.CorrelationId, actorUserId);

    var roleAuditLog = new AuditLog(user.TenantId, "RoleAssigned", "UserRole", userRole.Id.ToString(), roleAssignedEvent.CorrelationId, actorUserId, newValues: JsonSerializer.Serialize(new { userRole.UserId, userRole.RoleId }));

    await _dbContext.Users.AddAsync(user, cancellationToken);
    await _dbContext.UserRoles.AddAsync(userRole, cancellationToken);
    await _dbContext.AuditLogs.AddRangeAsync([userAuditLog, roleAuditLog], cancellationToken);
    await _dbContext.OutboxMessages.AddRangeAsync([userCreatedOutbox, roleAssignedOutbox], cancellationToken);

    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task<bool> HasRoleAssignmentInTenantAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken)
  {
    return (
      from user in _dbContext.Users
      join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
      where user.TenantId == tenantId && userRole.RoleId == roleId
      select userRole.Id
    ).AnyAsync(cancellationToken);
  }

  public async Task UpdatePasswordAsync(User user, PasswordChangedIntegrationEvent passwordChangedEvent, string auditAction, CancellationToken cancellationToken)
  {
    var outboxMessage = new OutboxMessage(UserIntegrationEventTypes.PasswordChangedV1, JsonSerializer.Serialize(passwordChangedEvent), passwordChangedEvent.CorrelationId);

    var auditLog = new AuditLog(user.TenantId, auditAction, "User", user.Id.ToString(), passwordChangedEvent.CorrelationId, user.Id);

    await RevokeActiveRefreshTokensAsync(user.Id, cancellationToken);

    await _dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task<bool> HasActiveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
  {
    return _dbContext.UserRoles.AnyAsync(userRole => userRole.UserId == userId && userRole.RoleId == roleId && userRole.RevokedAtUtc == null, cancellationToken);
  }

  public Task<UserRole?> GetActiveUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
  {
    return _dbContext.UserRoles.FirstOrDefaultAsync(userRole => userRole.UserId == userId && userRole.RoleId == roleId && userRole.RevokedAtUtc == null, cancellationToken);
  }

  public async Task AddRoleAsync(User user, UserRole userRole, UserRoleAssignedIntegrationEvent roleAssignedEvent, Guid administratorUserId, CancellationToken cancellationToken)
  {
    var auditLog = new AuditLog(user.TenantId, "RoleAssigned", "UserRole", userRole.Id.ToString(), roleAssignedEvent.CorrelationId, administratorUserId, newValues: JsonSerializer.Serialize(new { user.Id, userRole.RoleId }));

    var outboxMessage = new OutboxMessage(UserIntegrationEventTypes.UserRoleAssignedV1, JsonSerializer.Serialize(roleAssignedEvent), roleAssignedEvent.CorrelationId);

    await _dbContext.UserRoles.AddAsync(userRole, cancellationToken);
    await _dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task RevokeRoleAsync(User user, UserRole userRole, UserRoleRevokedIntegrationEvent roleRevokedEvent, Guid administratorUserId, CancellationToken cancellationToken)
  {
    var auditLog = new AuditLog(user.TenantId, "RoleRevoked", "UserRole", userRole.Id.ToString(), roleRevokedEvent.CorrelationId, administratorUserId, oldValues: JsonSerializer.Serialize(new { user.Id, userRole.RoleId }));

    var outboxMessage = new OutboxMessage(UserIntegrationEventTypes.UserRoleRevokedV1, JsonSerializer.Serialize(roleRevokedEvent), roleRevokedEvent.CorrelationId);

    await _dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task ChangeStatusAsync(User user, UserStatus previousStatus, Guid administratorUserId, Guid correlationId, CancellationToken cancellationToken)
  {
    var auditLog = new AuditLog(user.TenantId, "UserStatusChanged", "User", user.Id.ToString(), correlationId, administratorUserId, oldValues: JsonSerializer.Serialize(new { Status = previousStatus }), newValues: JsonSerializer.Serialize(new { Status = user.Status }));

    if (user.Status != UserStatus.Active)
    {
      await RevokeActiveRefreshTokensAsync(user.Id, cancellationToken);
    }

    await _dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  private async Task RevokeActiveRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
  {
    var utcNow = DateTime.UtcNow;

    var refreshTokens = await _dbContext.RefreshTokens.Where(token => token.UserId == userId && token.RevokedAtUtc == null && token.ExpiresAtUtc > utcNow).ToListAsync(cancellationToken);

    foreach (var refreshToken in refreshTokens)
    {
      refreshToken.Revoke();
    }
  }
}
