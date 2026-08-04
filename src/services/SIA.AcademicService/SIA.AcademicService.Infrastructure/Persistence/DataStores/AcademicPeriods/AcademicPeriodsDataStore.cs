using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class AcademicPeriodsDataStore : IAcademicPeriodsDataStore
{
    private readonly AcademicDbContext _dbContext;

    public AcademicPeriodsDataStore(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> AcademicPeriodCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicPeriods.AnyAsync(academicPeriod => academicPeriod.TenantId == tenantId && academicPeriod.Code == code, cancellationToken);
    }

    public async Task AddAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);

        var eventType = $"{nameof(AcademicPeriodCreatedIntegrationEvent)}.v{integrationEvent.Version}";

        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.AcademicPeriods.AddAsync(academicPeriod, cancellationToken);

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

    public Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicPeriods.FirstOrDefaultAsync(academicPeriods => academicPeriods.Id == id, cancellationToken);
    }

    public async Task UpdateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent); 

        var eventType = $"{nameof(AcademicPeriodUpdatedIntegrationEvent)}.v{integrationEvent.Version}";

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

    public async Task DeactivateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodDeactivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);

        var eventType = $"{nameof(AcademicPeriodDeactivatedIntegrationEvent)}.v{integrationEvent.Version}";

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

    public async Task ActivateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodActivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);

        var eventType = $"{nameof(AcademicPeriodActivatedIntegrationEvent)}.v{integrationEvent.Version}";

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