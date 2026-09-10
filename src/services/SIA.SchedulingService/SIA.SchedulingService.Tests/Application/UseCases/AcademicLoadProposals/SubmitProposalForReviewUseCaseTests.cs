using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Contracts.Enums;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using DomainProposalStatus = SIA.SchedulingService.Domain.Enums.ProposalStatus;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadProposals;

public sealed class SubmitProposalForReviewUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidProposalAndAcademicLoads_ShouldSubmitForReview()
    {
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var proposal = CreateSampleProposal(tenantId, academicPeriodId, DomainProposalStatus.Draft);
        var proposalId = proposal.Id;

        var dataStore = new FakeProposalDataStore(proposal)
        {
            HasAcademicLoadsResult = true
        };
        var validator = new ProposalValidator(dataStore);
        var useCase = new SubmitProposalForReviewUseCase(dataStore, validator);

        var response = await useCase.ExecuteAsync(tenantId, proposalId, correlationId, CancellationToken.None);

        Assert.Equal(proposalId, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(ProposalStatus.SubmittedForReview, response.ProposalStatus);
        Assert.Equal(correlationId, response.CorrelationId);

        Assert.NotNull(dataStore.SubmittedProposal);
        Assert.Equal(proposalId, dataStore.SubmittedProposal.Id);
        Assert.Equal(DomainProposalStatus.SubmittedForReview, dataStore.SubmittedProposal.ProposalStatus);

        Assert.NotNull(dataStore.SubmittedIntegrationEvent);
        Assert.Equal(tenantId, dataStore.SubmittedIntegrationEvent.TenantId);
        Assert.Equal(proposalId, dataStore.SubmittedIntegrationEvent.ProposalId);
        Assert.Equal(proposal.EducationalProgramId, dataStore.SubmittedIntegrationEvent.EducationalProgramId);
        Assert.Equal(academicPeriodId, dataStore.SubmittedIntegrationEvent.AcademicPeriodId);
        Assert.Equal(proposal.DivisionHeadId, dataStore.SubmittedIntegrationEvent.DivisionHeadId);
        Assert.Equal(ProposalStatus.SubmittedForReview, dataStore.SubmittedIntegrationEvent.ProposalStatus);
        Assert.Equal(correlationId, dataStore.SubmittedIntegrationEvent.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalDoesNotExist_ShouldThrowProposalNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var dataStore = new FakeProposalDataStore(proposal: null);
        var validator = new ProposalValidator(dataStore);
        var useCase = new SubmitProposalForReviewUseCase(dataStore, validator);

        await Assert.ThrowsAsync<ProposalNotFoundException>(() => useCase.ExecuteAsync(tenantId, proposalId, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.SubmittedProposal);
        Assert.Null(dataStore.SubmittedIntegrationEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalIsNotInDraftStatus_ShouldThrowProposalNotEditableException()
    {
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();

        var proposal = CreateSampleProposal(tenantId, academicPeriodId, DomainProposalStatus.SubmittedForReview);
        var proposalId = proposal.Id;

        var dataStore = new FakeProposalDataStore(proposal);
        var validator = new ProposalValidator(dataStore);
        var useCase = new SubmitProposalForReviewUseCase(dataStore, validator);

        await Assert.ThrowsAsync<ProposalNotEditableException>(() => useCase.ExecuteAsync(tenantId, proposalId, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.SubmittedProposal);
        Assert.Null(dataStore.SubmittedIntegrationEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalHasNoAcademicLoads_ShouldThrowProposalNotValidForSubmissionException()
    {
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();

        var proposal = CreateSampleProposal(tenantId, academicPeriodId, DomainProposalStatus.Draft);
        var proposalId = proposal.Id;

        var dataStore = new FakeProposalDataStore(proposal)
        {
            HasAcademicLoadsResult = false
        };
        var validator = new ProposalValidator(dataStore);
        var useCase = new SubmitProposalForReviewUseCase(dataStore, validator);

        await Assert.ThrowsAsync<ProposalNotValidForSubmissionException>(() => useCase.ExecuteAsync(tenantId, proposalId, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.SubmittedProposal);
        Assert.Null(dataStore.SubmittedIntegrationEvent);
    }

    private static Proposal CreateSampleProposal(Guid tenantId, Guid academicPeriodId, DomainProposalStatus status)
    {
        var proposal = new Proposal(
            tenantId,
            Guid.NewGuid(),
            academicPeriodId,
            Guid.NewGuid()
        );

        if (status == DomainProposalStatus.SubmittedForReview)
        {
            proposal.SubmitForReview();
        }
        return proposal;
    }
}