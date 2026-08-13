using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;
using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class SupportScheduleDataStore : ISupportScheduleDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public SupportScheduleDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SupportSchedule?> GetSupportScheduleByIdAsync(Guid tenantId, Guid supportScheduleId, CancellationToken cancellationToken)
    {
        return _dbContext.SupportSchedules
            .Include(supportSchedule => supportSchedule.ClassroomLab)
            .FirstOrDefaultAsync(supportSchedule => supportSchedule.TenantId == tenantId && supportSchedule.Id == supportScheduleId, cancellationToken);
    }

    public async Task AddSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(SupportScheduleCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.SupportSchedules.AddAsync(supportSchedule, cancellationToken);
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

    public async Task UpdateSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(SupportScheduleUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.SupportSchedules.Update(supportSchedule);
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

    public async Task SoftDeleteSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(SupportScheduleDeletedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.SupportSchedules.Update(supportSchedule);
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

    public async Task RestoreSupportScheduleWithOutboxAsync(SupportSchedule supportSchedule, SupportScheduleRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(SupportScheduleRestoredIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.SupportSchedules.Update(supportSchedule);
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