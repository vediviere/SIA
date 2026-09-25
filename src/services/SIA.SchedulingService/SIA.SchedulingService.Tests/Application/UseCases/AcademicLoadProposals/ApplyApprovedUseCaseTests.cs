using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;
using SIA.SchedulingService.Tests.Common.Fakes;
using Microsoft.Extensions.Logging.Abstractions;


namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadProposals;

public sealed class ApplyApprovedUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidEvent_ShouldApproveProposal()
  {
    var tenantId = Guid.NewGuid();
    var proposal = CreateSubmittedProposal(tenantId);
    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      1,
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
    var useCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

    await Assert.ThrowsAsync<ProposalNotFoundException>(() =>
      useCase.ExecuteAsync(
        Guid.NewGuid(),
        Guid.NewGuid(),
        1,
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
    var useCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      1,
      eventId,
      "ProposalApprovedIntegrationEvent.v1",
      "SIA.WorkflowService",
      Guid.NewGuid(),
      CancellationToken.None);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      1,
      eventId,
      "ProposalApprovedIntegrationEvent.v1",
      "SIA.WorkflowService",
      Guid.NewGuid(),
      CancellationToken.None);

    Assert.Equal(ProposalStatus.Approved, proposal.ProposalStatus);
    Assert.Equal(1, dataStore.AppliedDecisionCount);
  }


    [Fact]
    public async Task ExecuteAsync_WithOlderVersion_ShouldDiscardWithoutModifyingProposal()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateSubmittedProposal(tenantId); 
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

        await useCase.ExecuteAsync(
          tenantId,
          proposal.Id,
          0, 
          Guid.NewGuid(),
          "ProposalApprovedIntegrationEvent.v1",
          "SIA.WorkflowService",
          Guid.NewGuid(),
          CancellationToken.None);

        Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
        Assert.Null(dataStore.AppliedDecisionProposal);
        Assert.Equal(0, dataStore.AppliedDecisionCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithFutureVersion_ShouldThrowProposalReviewVersionAheadException()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateSubmittedProposal(tenantId); /
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

        await Assert.ThrowsAsync<ProposalReviewVersionAheadException>(() =>
          useCase.ExecuteAsync(
            tenantId,
            proposal.Id,
            2, // versión futura
            Guid.NewGuid(),
            "ProposalApprovedIntegrationEvent.v1",
            "SIA.WorkflowService",
            Guid.NewGuid(),
            CancellationToken.None));

        Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
        Assert.Null(dataStore.AppliedDecisionProposal);
        Assert.Equal(0, dataStore.AppliedDecisionCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithOutOfOrderDelivery_ShouldKeepLatestVersionDecision()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateResubmittedProposal(tenantId); 
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

       
        await useCase.ExecuteAsync(
          tenantId,
          proposal.Id,
          2,
          Guid.NewGuid(),
          "ProposalApprovedIntegrationEvent.v1",
          "SIA.WorkflowService",
          Guid.NewGuid(),
          CancellationToken.None);

        
        await useCase.ExecuteAsync(
          tenantId,
          proposal.Id,
          1,
          Guid.NewGuid(),
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

    private static Proposal CreateResubmittedProposal(Guid tenantId)
    {
        var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        proposal.SubmitForReview();    
        proposal.RequireCorrection(); 
        proposal.SubmitForReview();   

        return proposal;
    }
}
