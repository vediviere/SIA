using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Interfaces.DataStores;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;

public sealed class ApplyApprovedUseCase
{
    private readonly IProposalDataStore _dataStore;

    public ApplyApprovedUseCase(IProposalDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid proposalId, Guid eventId, string eventType, string sourceService, Guid correlationId, CancellationToken cancellationToken)
    {
        if (await _dataStore.WasProposalDecisionProcessedAsync(eventId, cancellationToken))
        {
            return;
        }

        var proposal = await _dataStore.GetByIdAsync(tenantId, proposalId, cancellationToken);

        if (proposal is null)
        {
            throw new ProposalNotFoundException(proposalId);
        }
        proposal.Approve();

        await _dataStore.ApplyDecisionAsync(proposal, eventId, eventType, sourceService, correlationId, cancellationToken);
    }
}