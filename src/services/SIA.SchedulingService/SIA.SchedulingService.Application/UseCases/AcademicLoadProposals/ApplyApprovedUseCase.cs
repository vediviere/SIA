using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using Microsoft.Extensions.Logging;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;

public sealed class ApplyApprovedUseCase
{
    private readonly IProposalDataStore _dataStore;
    private readonly ILogger<ApplyApprovedUseCase> _logger;

    public ApplyApprovedUseCase(IProposalDataStore dataStore, ILogger<ApplyApprovedUseCase> logger)
    {
        _dataStore = dataStore;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid proposalId, int version,  Guid eventId, string eventType, string sourceService, Guid correlationId, CancellationToken cancellationToken)
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

        if (version < proposal.ReviewVersion)
        {
            _logger.LogWarning(
                "Decisión descartada por versión anterior. EventId {EventId}, ProposalId {ProposalId}, versión recibida {ReceivedVersion}, versión vigente {CurrentVersion}, CorrelationId {CorrelationId}.",
                eventId, proposalId, version, proposal.ReviewVersion, correlationId);
            return;
        }

        if (version > proposal.ReviewVersion)
        {
            throw new ProposalReviewVersionAheadException(proposalId, version, proposal.ReviewVersion);
        }

        proposal.Approve();

        await _dataStore.ApplyDecisionAsync(proposal, eventId, eventType, sourceService, correlationId, cancellationToken);
    }
}