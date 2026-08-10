using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class ClassroomTypeDataStore : IClassroomTypeDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public ClassroomTypeDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public  Task<bool> ClassroomTypeNameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken)
    {
        return _dbContext.ClassroomTypes.AnyAsync(
            classroomType => classroomType.TenantId == tenantId && classroomType.Name == name,
            cancellationToken);
    }

    public Task<ClassroomType?> GetClassroomTypeByIdAsync(Guid tenantId, Guid classroomTypeId, CancellationToken cancellationToken)
    {
        return _dbContext.ClassroomTypes.FirstOrDefaultAsync(
            classroomType => classroomType.TenantId == tenantId && classroomType.Id == classroomTypeId,
            cancellationToken);
    }

    public async Task AddClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomTypeCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.ClassroomTypes.AddAsync(classroomType, cancellationToken);
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

    public async Task UpdateClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomTypeUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassroomTypes.Update(classroomType);
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

    public async Task SoftDeleteClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomTypeDeletedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassroomTypes.Update(classroomType);
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

    public async Task RestoreClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(ClassroomTypeRestoredIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.ClassroomTypes.Update(classroomType);
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