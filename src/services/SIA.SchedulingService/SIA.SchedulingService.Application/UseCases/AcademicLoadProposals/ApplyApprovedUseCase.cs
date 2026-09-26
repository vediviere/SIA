using Microsoft.Extensions.Logging;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;

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

        var hasAcademicLoads = await _dataStore.HasAcademicLoadsAsync(tenantId, proposalId, cancellationToken);
        if (!hasAcademicLoads)
        {
            throw new ProposalNotValidForApprovalException(proposalId);
        }
        proposal.Approve();
        var occurredAtUtc = proposal.UpdatedAtUtc!.Value;

        var academicLoadApprovedEvent = new AcademicLoadApprovedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = occurredAtUtc,
            TenantId = proposal.TenantId,
            ProposalId = proposal.Id,
            EducationalProgramId = proposal.EducationalProgramId,
            AcademicPeriodId = proposal.AcademicPeriodId,
            DivisionHeadId = proposal.DivisionHeadId,
            Version = 1
        };
        await _dataStore.ProposalApprovalWithOutboxAsync(
            proposal,
            eventId,
            eventType,
            sourceService,
            correlationId,
            academicLoadApprovedEvent,
            cancellationToken);
    }
}