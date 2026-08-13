using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;

using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class ClassScheduleDataStore : IClassScheduleDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public ClassScheduleDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClassSchedule?> GetClassScheduleByIdAsync(Guid tenantId, Guid classScheduleId, CancellationToken cancellationToken)
    {
        return await _dbContext.ClassSchedules
            .Include(x => x.ClassroomLab)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == classScheduleId, cancellationToken);
    }

    public async Task AddClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassScheduleCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.ClassSchedules.AddAsync(classSchedule, cancellationToken);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassScheduleUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassSchedules.Update(classSchedule);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task SoftDeleteClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassScheduleDeletedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassSchedules.Update(classSchedule);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RestoreClassScheduleWithOutboxAsync(
        ClassSchedule classSchedule,
        ClassScheduleRestoredIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassScheduleRestoredIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassSchedules.Update(classSchedule);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}