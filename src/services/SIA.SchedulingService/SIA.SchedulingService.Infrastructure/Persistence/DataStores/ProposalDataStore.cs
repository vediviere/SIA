using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using OutboxMessage = SIA.BuildingBlocks.Messaging.Outbox.OutboxMessage;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.Entities;

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

  public Task<bool> HasAcademicLoadsAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicLoad.AnyAsync(load => load.TenantId == tenantId && load.ProposalId == proposalId && load.Status,cancellationToken);
  }
  public async Task SubmitForReviewWithOutboxAsync(Proposal proposal,ProposalSubmittedForReviewIntegrationEvent integrationEvent,CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.ProposalSubmittedForReviewV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.AcademicLoadProposals.Update(proposal);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task<bool> WasProposalDecisionProcessedAsync(Guid eventId, CancellationToken cancellationToken)
  {
    return _dbContext.InboxMessages.AsNoTracking().AnyAsync(message => message.Id == eventId, cancellationToken);
  }

  public async Task ApplyDecisionAsync(Proposal proposal, Guid eventId, string eventType, string sourceService, Guid correlationId, CancellationToken cancellationToken)
  {
    var inboxMessage = new InboxMessage(eventId, eventType, sourceService, correlationId);
    inboxMessage.MarkAsProcessed();

    _dbContext.AcademicLoadProposals.Update(proposal);
    await _dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task ProposalApprovalWithOutboxAsync(Proposal proposal, Guid eventId, string eventType, string sourceService, Guid correlationId, AcademicLoadApprovedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var outboxEventType = SchedulingIntegrationEventTypes.AcademicLoadApprovedV1;
    var outboxMessage = new OutboxMessage(outboxEventType, payload, integrationEvent.CorrelationId);

    var inboxMessage = new InboxMessage(eventId, eventType, sourceService, correlationId);
    inboxMessage.MarkAsProcessed();

    _dbContext.AcademicLoadProposals.Update(proposal);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}