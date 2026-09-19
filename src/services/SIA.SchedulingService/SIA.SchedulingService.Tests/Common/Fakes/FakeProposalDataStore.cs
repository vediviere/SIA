using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeProposalDataStore : IProposalDataStore
{
  private readonly Proposal? _proposal;
  private readonly HashSet<Guid> _processedEventIds = [];

  public FakeProposalDataStore(Proposal? proposal = null)
  {
    _proposal = proposal;
  }

  public bool ExistsResult { get; set; }
  public bool HasAcademicLoadsResult { get; set; } = true;
  public Proposal? AddedProposal { get; private set; }
  public ProposalCreatedIntegrationEvent? AddedCreatedEvent { get; private set; }

  public int ApplyDecisionCallCount { get; private set; }
  public Guid? LastAppliedEventId { get; private set; }
  public Guid? LastAppliedCorrelationId { get; private set; }

  public Proposal? SubmittedProposal { get; private set; }
  public ProposalSubmittedForReviewIntegrationEvent? SubmittedIntegrationEvent { get; private set; }

  public bool DecisionProcessedResult { get; set; }
  public Proposal? AppliedDecisionProposal { get; private set; }
  public Guid? AppliedEventId { get; private set; }
  public string? AppliedEventType { get; private set; }
  public string? AppliedSourceService { get; private set; }
  public Guid? AppliedCorrelationId { get; private set; }
  public int AppliedDecisionCount { get; private set; }

  public Task<Proposal?> GetByIdAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken)
  {
    if (_proposal is null || _proposal.TenantId != tenantId || _proposal.Id != proposalId)
    {
      return Task.FromResult<Proposal?>(null);
    }

    return Task.FromResult<Proposal?>(_proposal);
  }

  public Task<bool> ExistsAsync(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, CancellationToken cancellationToken)
  {
    return Task.FromResult(ExistsResult);
  }

  public Task AddWithOutboxAsync(Proposal proposal, ProposalCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    AddedProposal = proposal;
    AddedCreatedEvent = integrationEvent;
    return Task.CompletedTask;
  }

  public Task<bool> HasAcademicLoadsAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken)
  {
    return Task.FromResult(HasAcademicLoadsResult);
  }

  public Task SubmitForReviewWithOutboxAsync(Proposal proposal, ProposalSubmittedForReviewIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    SubmittedProposal = proposal;
    SubmittedIntegrationEvent = integrationEvent;
    return Task.CompletedTask;
  }

  public Task<bool> WasProposalDecisionProcessedAsync(Guid eventId, CancellationToken cancellationToken)
{
  return Task.FromResult(DecisionProcessedResult || _processedEventIds.Contains(eventId));
}

  public Task ApplyDecisionAsync(Proposal proposal, Guid eventId, string eventType, string sourceService, Guid correlationId, CancellationToken cancellationToken)
  {
    AppliedDecisionProposal = proposal;
    AppliedEventId = eventId;
    AppliedEventType = eventType;
    AppliedSourceService = sourceService;
    AppliedCorrelationId = correlationId;
    AppliedDecisionCount++;

    _processedEventIds.Add(eventId);
    ApplyDecisionCallCount++;
    LastAppliedEventId = eventId;
    LastAppliedCorrelationId = correlationId;

    return Task.CompletedTask;
  }
}
