using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeProposalDataStore : IProposalDataStore
{
  private readonly Proposal? _proposal;

  public FakeProposalDataStore(Proposal? proposal = null)
  {
    _proposal = proposal;
  }

  public bool ExistsResult { get; set; }
  public Proposal? AddedProposal { get; private set; }
  public ProposalCreatedIntegrationEvent? AddedCreatedEvent { get; private set; }

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
}
