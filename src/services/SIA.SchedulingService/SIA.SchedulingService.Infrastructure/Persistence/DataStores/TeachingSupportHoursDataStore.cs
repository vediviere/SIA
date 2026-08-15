using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class TeachingSupportHoursDataStore : ITeachingSupportHoursDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public TeachingSupportHoursDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByActivityAndAcademicLoadAsync(Guid activityId, Guid academicLoadId, CancellationToken cancellationToken)
    {
        return _dbContext.TeachingSupportHours.AnyAsync(hours => hours.ActivityId == activityId && hours.AcademicLoadId == academicLoadId, cancellationToken);
    }

    public async Task AddTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursCreatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeachingSupportHoursCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.TeachingSupportHours.AddAsync(teachingSupportHours, cancellationToken);
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

    public Task<Domain.Entities.TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.TeachingSupportHours.FirstOrDefaultAsync(hours => hours.TenantId == tenantId && hours.Id == id, cancellationToken);
    }

    public async Task UpdateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeachingSupportHoursUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
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

    public async Task DeactivateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursDeactivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeachingSupportHoursDeactivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
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

    public async Task ActivateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursActivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeachingSupportHoursActivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
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