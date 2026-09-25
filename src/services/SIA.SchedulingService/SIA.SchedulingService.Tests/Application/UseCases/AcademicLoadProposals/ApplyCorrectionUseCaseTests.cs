using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;
using SIA.SchedulingService.Tests.Common.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadProposals;

public sealed class ApplyCorrectionUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_FromSubmitted_ShouldRequireCorrection()
  {
    var tenantId = Guid.NewGuid();
    var eventId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var proposal = CreateSubmittedProposal(tenantId);
    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      1,
      eventId,
      "ProposalRequiresCorrectionIntegrationEvent.v1",
      "SIA.WorkflowService",
      correlationId,
      CancellationToken.None);

    Assert.Equal(ProposalStatus.RequiresCorrection, proposal.ProposalStatus);
    Assert.Same(proposal, dataStore.AppliedDecisionProposal);
    Assert.Equal(eventId, dataStore.AppliedEventId);
    Assert.Equal("ProposalRequiresCorrectionIntegrationEvent.v1", dataStore.AppliedEventType);
    Assert.Equal("SIA.WorkflowService", dataStore.AppliedSourceService);
    Assert.Equal(correlationId, dataStore.AppliedCorrelationId);
    Assert.Equal(1, dataStore.AppliedDecisionCount);
  }

  [Fact]
  public async Task ExecuteAsync_FromApproved_ShouldThrowInvalidOperationException()
  {
    var tenantId = Guid.NewGuid();
    var proposal = CreateSubmittedProposal(tenantId);
    proposal.Approve();

    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);

    await Assert.ThrowsAsync<InvalidOperationException>(() =>
      useCase.ExecuteAsync(
        tenantId,
        proposal.Id,
        1,
        Guid.NewGuid(),
        "ProposalRequiresCorrectionIntegrationEvent.v1",
        "SIA.WorkflowService",
        Guid.NewGuid(),
        CancellationToken.None));

    Assert.Equal(ProposalStatus.Approved, proposal.ProposalStatus);
    Assert.Null(dataStore.AppliedDecisionProposal);
    Assert.Equal(0, dataStore.AppliedDecisionCount);
  }

  [Fact]
  public async Task ExecuteAsync_WithDuplicatedEvent_ShouldBeIdempotent()
  {
    var tenantId = Guid.NewGuid();
    var eventId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var proposal = CreateSubmittedProposal(tenantId);
    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      1,
      eventId,
      "ProposalRequiresCorrectionIntegrationEvent.v1",
      "SIA.WorkflowService",
      correlationId,
      CancellationToken.None);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      1,
      eventId,
      "ProposalRequiresCorrectionIntegrationEvent.v1",
      "SIA.WorkflowService",
      correlationId,
      CancellationToken.None);

    Assert.Equal(ProposalStatus.RequiresCorrection, proposal.ProposalStatus);
    Assert.Same(proposal, dataStore.AppliedDecisionProposal);
    Assert.Equal(1, dataStore.AppliedDecisionCount);
  }

  [Fact]
  public async Task ExecuteAsync_WithDifferentTenant_ShouldNotModifyProposal()
  {
    var proposal = CreateSubmittedProposal(Guid.NewGuid());
    var dataStore = new FakeProposalDataStore(proposal);
    var useCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);

    await Assert.ThrowsAsync<ProposalNotFoundException>(() =>
      useCase.ExecuteAsync(
        Guid.NewGuid(),
        proposal.Id,
        1,
        Guid.NewGuid(),
        "ProposalRequiresCorrectionIntegrationEvent.v1",
        "SIA.WorkflowService",
        Guid.NewGuid(),
        CancellationToken.None));

    Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
    Assert.Null(dataStore.AppliedDecisionProposal);
    Assert.Equal(0, dataStore.AppliedDecisionCount);
  }

    [Fact]
    public async Task ExecuteAsync_WithOlderVersion_ShouldDiscardWithoutModifyingProposal()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateSubmittedProposal(tenantId);
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);

        await useCase.ExecuteAsync(
          tenantId,
          proposal.Id,
          0,
          Guid.NewGuid(),
          "ProposalRequiresCorrectionIntegrationEvent.v1",
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
        var proposal = CreateSubmittedProposal(tenantId);
        var dataStore = new FakeProposalDataStore(proposal);
        var useCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);

        await Assert.ThrowsAsync<ProposalReviewVersionAheadException>(() =>
          useCase.ExecuteAsync(
            tenantId,
            proposal.Id,
            2,
            Guid.NewGuid(),
            "ProposalRequiresCorrectionIntegrationEvent.v1",
            "SIA.WorkflowService",
            Guid.NewGuid(),
            CancellationToken.None));

        Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
        Assert.Null(dataStore.AppliedDecisionProposal);
        Assert.Equal(0, dataStore.AppliedDecisionCount);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnedCorrectedAndResubmitted_LateApprovalOfOldVersion_ShouldBeDiscarded()
    {
        var tenantId = Guid.NewGuid();
        var proposal = CreateSubmittedProposal(tenantId); 
        var dataStore = new FakeProposalDataStore(proposal);
        var correctionUseCase = new ApplyCorrectionUseCase(dataStore, NullLogger<ApplyCorrectionUseCase>.Instance);
        var approvedUseCase = new ApplyApprovedUseCase(dataStore, NullLogger<ApplyApprovedUseCase>.Instance);

     
        await correctionUseCase.ExecuteAsync(
          tenantId, proposal.Id, 1, Guid.NewGuid(),
          "ProposalRequiresCorrectionIntegrationEvent.v1", "SIA.WorkflowService", Guid.NewGuid(),
          CancellationToken.None);

        Assert.Equal(ProposalStatus.RequiresCorrection, proposal.ProposalStatus);

        
        proposal.SubmitForReview();
        Assert.Equal(2, proposal.ReviewVersion);

        
        await approvedUseCase.ExecuteAsync(
          tenantId, proposal.Id, 1, Guid.NewGuid(),
          "ProposalApprovedIntegrationEvent.v1", "SIA.WorkflowService", Guid.NewGuid(),
          CancellationToken.None);

        Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus); 
        Assert.Equal(1, dataStore.AppliedDecisionCount);
    }

    private static Proposal CreateSubmittedProposal(Guid tenantId)
  {
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    proposal.SubmitForReview();

    return proposal;
  }
}
