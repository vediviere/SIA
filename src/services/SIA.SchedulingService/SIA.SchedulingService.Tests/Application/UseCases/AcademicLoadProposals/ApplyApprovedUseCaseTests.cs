using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadProposals;

public sealed class ApplyApprovedUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidEvent_ShouldApproveProposal()
  {
    var tenantId = Guid.NewGuid();
    var proposal = CreateSubmittedProposal(tenantId);
    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyApprovedUseCase(dataStore);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      Guid.NewGuid(),
      "ProposalApprovedIntegrationEvent.v1",
      "SIA.WorkflowService",
      Guid.NewGuid(),
      CancellationToken.None);

    Assert.Equal(ProposalStatus.Approved, proposal.ProposalStatus);
    Assert.Same(proposal, dataStore.AppliedDecisionProposal);
    Assert.Equal(1, dataStore.AppliedDecisionCount);
  }

  [Fact]
  public async Task ExecuteAsync_WhenProposalDoesNotExist_ShouldThrowProposalNotFoundException()
  {
    var dataStore = new FakeProposalDataStore();
    var useCase = new ApplyApprovedUseCase(dataStore);

    await Assert.ThrowsAsync<ProposalNotFoundException>(() =>
      useCase.ExecuteAsync(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        "ProposalApprovedIntegrationEvent.v1",
        "SIA.WorkflowService",
        Guid.NewGuid(),
        CancellationToken.None));

    Assert.Null(dataStore.AppliedDecisionProposal);
    Assert.Equal(0, dataStore.AppliedDecisionCount);
  }

  [Fact]
  public async Task ExecuteAsync_WhenEventAlreadyProcessed_ShouldNotApplyDecisionTwice()
  {
    var tenantId = Guid.NewGuid();
    var eventId = Guid.NewGuid();
    var proposal = CreateSubmittedProposal(tenantId);
    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyApprovedUseCase(dataStore);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      eventId,
      "ProposalApprovedIntegrationEvent.v1",
      "SIA.WorkflowService",
      Guid.NewGuid(),
      CancellationToken.None);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      eventId,
      "ProposalApprovedIntegrationEvent.v1",
      "SIA.WorkflowService",
      Guid.NewGuid(),
      CancellationToken.None);

    Assert.Equal(ProposalStatus.Approved, proposal.ProposalStatus);
    Assert.Equal(1, dataStore.AppliedDecisionCount);
  }

  private static Proposal CreateSubmittedProposal(Guid tenantId)
  {
    var proposal = new Proposal(
      tenantId,
      Guid.NewGuid(),
      Guid.NewGuid(),
      Guid.NewGuid());

    proposal.SubmitForReview();

    return proposal;
  }
}
