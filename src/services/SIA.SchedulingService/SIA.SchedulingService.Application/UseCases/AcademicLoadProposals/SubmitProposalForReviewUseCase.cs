using ContractProposalStatus = SIA.SchedulingService.Contracts.Enums.ProposalStatus;
using DomainProposalStatus = SIA.SchedulingService.Domain.Enums.ProposalStatus;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadProposal;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;

public sealed class SubmitProposalForReviewUseCase
{
    private readonly IProposalDataStore _dataStore;
    private readonly ProposalValidator _validator;

    public SubmitProposalForReviewUseCase(IProposalDataStore dataStore, ProposalValidator validator)
    {
        _dataStore = dataStore;
        _validator = validator;
    }

    public async Task<SubmitProposalForReviewResponse> ExecuteAsync(Guid tenantId, Guid proposalId, Guid correlationId, CancellationToken cancellationToken)
    {
        var proposal = await _dataStore.GetByIdAsync(tenantId, proposalId, cancellationToken);

        if (proposal is null)
        {
            throw new ProposalNotFoundException(proposalId);
        }

        await _validator.EnsureEditableAsync(tenantId, proposalId, proposal.AcademicPeriodId, cancellationToken);



        var hasAcademicLoads = await _dataStore.HasAcademicLoadsAsync(tenantId, proposalId, cancellationToken);

        if (!hasAcademicLoads)
        {
            throw new ProposalNotValidForSubmissionException(proposalId);
        }



        proposal.SubmitForReview();

        var proposalStatus = MapToContractStatus(proposal.ProposalStatus);

        var integrationEvent = new ProposalSubmittedForReviewIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = proposal.UpdatedAtUtc!.Value,
            TenantId = proposal.TenantId,
            ProposalId = proposal.Id,
            EducationalProgramId = proposal.EducationalProgramId,
            AcademicPeriodId = proposal.AcademicPeriodId,
            DivisionHeadId = proposal.DivisionHeadId,
            ProposalStatus = proposalStatus,
            Status = proposal.Status,
            Version = 1
        };

        await _dataStore.SubmitForReviewWithOutboxAsync(proposal, integrationEvent, cancellationToken);

        return new SubmitProposalForReviewResponse
        {
            Id = proposal.Id,
            TenantId = proposal.TenantId,
            EducationalProgramId = proposal.EducationalProgramId,
            AcademicPeriodId = proposal.AcademicPeriodId,
            DivisionHeadId = proposal.DivisionHeadId,
            ProposalStatus = proposalStatus,
            Status = proposal.Status,
            CreatedAtUtc = proposal.CreatedAtUtc,
            UpdatedAtUtc = proposal.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }

    private static ContractProposalStatus MapToContractStatus(DomainProposalStatus status) => status switch
    {
        DomainProposalStatus.Draft => ContractProposalStatus.Draft,
        DomainProposalStatus.SubmittedForReview => ContractProposalStatus.SubmittedForReview,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado de propuesta sin mapeo definido hacia el contrato.")
    };
}