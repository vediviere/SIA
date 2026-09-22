using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;
using SIA.SchedulingService.Tests.Common.Fakes;

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
    var useCase = new ApplyCorrectionUseCase(dataStore);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
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
    var useCase = new ApplyCorrectionUseCase(dataStore);

    await Assert.ThrowsAsync<InvalidOperationException>(() =>
      useCase.ExecuteAsync(
        tenantId,
        proposal.Id,
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
    var useCase = new ApplyCorrectionUseCase(dataStore);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
      eventId,
      "ProposalRequiresCorrectionIntegrationEvent.v1",
      "SIA.WorkflowService",
      correlationId,
      CancellationToken.None);

    await useCase.ExecuteAsync(
      tenantId,
      proposal.Id,
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
    var useCase = new ApplyCorrectionUseCase(dataStore);

    await Assert.ThrowsAsync<ProposalNotFoundException>(() =>
      useCase.ExecuteAsync(
        Guid.NewGuid(),
        proposal.Id,
        Guid.NewGuid(),
        "ProposalRequiresCorrectionIntegrationEvent.v1",
        "SIA.WorkflowService",
        Guid.NewGuid(),
        CancellationToken.None));

    Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
    Assert.Null(dataStore.AppliedDecisionProposal);
    Assert.Equal(0, dataStore.AppliedDecisionCount);
  }

  private static Proposal CreateSubmittedProposal(Guid tenantId)
  {
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    proposal.SubmitForReview();

    return proposal;
  }
}
