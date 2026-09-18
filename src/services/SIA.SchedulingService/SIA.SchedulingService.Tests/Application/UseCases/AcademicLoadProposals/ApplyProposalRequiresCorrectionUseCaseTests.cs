using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadProposals;

public sealed class ApplyProposalRequiresCorrectionUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidEvent_ShouldRejectProposal()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateSubmittedProposal(tenantId);
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyProposalRequiresCorrectionUseCase(dataStore);

        await useCase.ExecuteAsync(tenantId, proposal.Id, Guid.NewGuid(), "ProposalRequiresCorrectionIntegrationEvent.v1", "SIA.WorkflowService", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(ProposalStatus.Rejected, proposal.ProposalStatus);
        Assert.Equal(1, dataStore.ApplyDecisionCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalDoesNotExist_ShouldThrowProposalNotFoundException()
    {
        var dataStore = new FakeProposalDataStore(proposal: null);
        var useCase = new ApplyProposalRequiresCorrectionUseCase(dataStore);

        await Assert.ThrowsAsync<ProposalNotFoundException>(() =>
          useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ProposalRequiresCorrectionIntegrationEvent.v1", "SIA.WorkflowService", Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenEventAlreadyProcessed_ShouldNotApplyDecisionTwice()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateSubmittedProposal(tenantId);
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyProposalRequiresCorrectionUseCase(dataStore);
        var eventId = Guid.NewGuid();

        await useCase.ExecuteAsync(tenantId, proposal.Id, eventId, "ProposalRequiresCorrectionIntegrationEvent.v1", "SIA.WorkflowService", Guid.NewGuid(), CancellationToken.None);
        await useCase.ExecuteAsync(tenantId, proposal.Id, eventId, "ProposalRequiresCorrectionIntegrationEvent.v1", "SIA.WorkflowService", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(ProposalStatus.Rejected, proposal.ProposalStatus);
        Assert.Equal(1, dataStore.ApplyDecisionCallCount);
    }

    private static Proposal CreateSubmittedProposal(Guid tenantId)
    {
        var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        proposal.SubmitForReview();
        return proposal;
    }
}