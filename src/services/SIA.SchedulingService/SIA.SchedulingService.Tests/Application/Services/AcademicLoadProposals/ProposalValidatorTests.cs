using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.Common.Services.AcademicLoadProposals;

public sealed class ProposalValidatorTests
{
  [Fact]
  public async Task EnsureEditableAsync_WithDraftProposal_ShouldNotThrow()
  {
    var tenantId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var validator = new ProposalValidator(new FakeProposalDataStore(proposal));

    await validator.EnsureEditableAsync(tenantId, proposal.Id, proposal.AcademicPeriodId, CancellationToken.None);
  }

  [Fact]
  public async Task EnsureEditableAsync_WhenProposalDoesNotExist_ShouldThrowProposalNotEditableException()
  {
    var validator = new ProposalValidator(new FakeProposalDataStore());

    await Assert.ThrowsAsync<ProposalNotEditableException>(() =>
      validator.EnsureEditableAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task EnsureEditableAsync_WhenAcademicPeriodDoesNotMatch_ShouldThrowProposalNotEditableException()
  {
    var tenantId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var validator = new ProposalValidator(new FakeProposalDataStore(proposal));

    await Assert.ThrowsAsync<ProposalNotEditableException>(() =>
      validator.EnsureEditableAsync(tenantId, proposal.Id, Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task EnsureEditableAsync_WhenAcademicLoadIsInactive_ShouldThrowAcademicLoadNotEditableException()
  {
    var tenantId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    academicLoad.Deactivate();

    var validator = new ProposalValidator(new FakeProposalDataStore(proposal));

    await Assert.ThrowsAsync<AcademicLoadNotEditableException>(() =>
      validator.EnsureEditableAsync(academicLoad, CancellationToken.None));
  }
}
