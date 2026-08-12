using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.AcademicStaffService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.DataStores;

public sealed class DivisionHeadDataStore : IDivisionHeadDataStore
{
    private readonly AcademicStaffDbContext _dbContext;

    public DivisionHeadDataStore(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> PersonAlreadyManagesProgramAsync(Guid tenantId, Guid programId, Guid personId, CancellationToken cancellationToken)
    {
        return _dbContext.DivisionHead.AnyAsync(
            divisionHead =>
                divisionHead.TenantId == tenantId &&
                divisionHead.ProgramId == programId &&
                divisionHead.PersonId == personId,
            cancellationToken);
    }

    public Task<DivisionHead?> GetDivisionManagerByIdAsync(Guid tenantId, Guid divisionManagerId, CancellationToken cancellationToken)
    {
        return _dbContext.DivisionHead.FirstOrDefaultAsync(
            divisionHead => divisionHead.TenantId == tenantId && divisionHead.Id == divisionManagerId,
            cancellationToken);
    }

    public async Task AddDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(DivisionHeadCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.DivisionHead.AddAsync(divisionHead, cancellationToken);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(DivisionHeadUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.DivisionHead.Update(divisionHead);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task ActivateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(DivisionHeadActivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.DivisionHead.Update(divisionHead);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeactivateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(DivisionHeadDeactivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.DivisionHead.Update(divisionHead);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
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