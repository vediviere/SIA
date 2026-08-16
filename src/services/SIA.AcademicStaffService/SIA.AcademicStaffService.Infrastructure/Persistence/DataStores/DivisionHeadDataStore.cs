using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

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
        return _dbContext.DivisionHeads.AnyAsync(
            divisionHead =>
                divisionHead.TenantId == tenantId &&
                divisionHead.ProgramId == programId &&
                divisionHead.PersonId == personId,
            cancellationToken);
    }

    public Task<DivisionHead?> GetDivisionManagerByIdAsync(Guid tenantId, Guid divisionManagerId, CancellationToken cancellationToken)
    {
        return _dbContext.DivisionHeads.FirstOrDefaultAsync(
            divisionHead => divisionHead.TenantId == tenantId && divisionHead.Id == divisionManagerId,
            cancellationToken);
    }

    public async Task AddDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.DivisionHeadCreatedV1, payload, integrationEvent.CorrelationId);

        await _dbContext.DivisionHeads.AddAsync(divisionHead, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }



    public async Task ActivateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.DivisionHeadActivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.DivisionHeads.Update(divisionHead);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateDivisionManagerWithOutboxAsync(DivisionHead divisionHead, DivisionHeadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.DivisionHeadDeactivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.DivisionHeads.Update(divisionHead);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}