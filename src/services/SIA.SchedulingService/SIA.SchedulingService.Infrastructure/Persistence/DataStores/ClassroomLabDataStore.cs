using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class ClassroomLabDataStore : IClassroomLabDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public ClassroomLabDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ClassroomLabCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return _dbContext.ClassroomLabs.AnyAsync(
            classroomLab => classroomLab.TenantId == tenantId && classroomLab.Code == code,
            cancellationToken);
    }

    public Task<ClassroomLab?> GetClassroomLabByIdAsync(Guid tenantId, Guid classroomLabId, CancellationToken cancellationToken)
    {
        return _dbContext.ClassroomLabs
            .Include(classroomLab => classroomLab.ClassroomType)
            .FirstOrDefaultAsync(classroomLab => classroomLab.TenantId == tenantId && classroomLab.Id == classroomLabId, cancellationToken);
    }

    public async Task AddClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomLabCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.ClassroomLabs.AddAsync(classroomLab, cancellationToken);
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

    public async Task UpdateClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomLabUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassroomLabs.Update(classroomLab);
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

    public async Task SoftDeleteClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomLabDeletedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassroomLabs.Update(classroomLab);
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

    public async Task RestoreClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomLabRestoredIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassroomLabs.Update(classroomLab);
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