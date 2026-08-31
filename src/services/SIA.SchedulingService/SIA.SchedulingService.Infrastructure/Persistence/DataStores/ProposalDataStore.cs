using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class ProposalDataStore : IProposalDataStore
{
  private readonly SchedulingDbContext _dbContext;

  public ProposalDataStore(SchedulingDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<Proposal?> GetByIdAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicLoadProposals.FirstOrDefaultAsync(proposal => proposal.TenantId == tenantId && proposal.Id == proposalId, cancellationToken);
  }

  public Task<bool> ExistsAsync(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicLoadProposals.AnyAsync(proposal =>
      proposal.TenantId == tenantId &&
      proposal.EducationalProgramId == educationalProgramId &&
      proposal.AcademicPeriodId == academicPeriodId &&
      proposal.Status, cancellationToken);
  }

  public async Task AddWithOutboxAsync(Proposal proposal, ProposalCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.ProposalCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.AcademicLoadProposals.AddAsync(proposal, cancellationToken);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
