using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;

namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class ProposalTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateDraftProposal()
  {
    var tenantId = Guid.NewGuid();
    var educationalProgramId = Guid.NewGuid();
    var academicPeriodId = Guid.NewGuid();
    var divisionHeadId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, educationalProgramId, academicPeriodId, divisionHeadId);

    Assert.NotEqual(Guid.Empty, proposal.Id);
    Assert.Equal(tenantId, proposal.TenantId);
    Assert.Equal(educationalProgramId, proposal.EducationalProgramId);
    Assert.Equal(academicPeriodId, proposal.AcademicPeriodId);
    Assert.Equal(divisionHeadId, proposal.DivisionHeadId);
    Assert.Equal(ProposalStatus.Draft, proposal.ProposalStatus);
    Assert.True(proposal.Status);
    Assert.Null(proposal.UpdatedAtUtc);
    Assert.Equal(0, proposal.ReviewVersion);
  }

  [Fact]
  public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new Proposal(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyEducationalProgramId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new Proposal(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyAcademicPeriodId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyDivisionHeadId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty));
  }

  [Fact]
  public void RequireCorrection_WhenSubmittedForReview_ShouldChangeStatus()
  {
    var proposal = new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    proposal.SubmitForReview();

    proposal.RequireCorrection();

    Assert.Equal(ProposalStatus.RequiresCorrection, proposal.ProposalStatus);
    Assert.NotNull(proposal.UpdatedAtUtc);
  }

  [Fact]
  public void RequireCorrection_WhenApproved_ShouldThrowInvalidOperationException()
  {
    var proposal = new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    proposal.SubmitForReview();
    proposal.Approve();

    Assert.Throws<InvalidOperationException>(() => proposal.RequireCorrection());
    Assert.Equal(ProposalStatus.Approved, proposal.ProposalStatus);
  }

  [Fact]
  public void SubmitForReview_WhenRequiresCorrection_ShouldSubmitAgain()
  {
    var proposal = new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    proposal.SubmitForReview();
    proposal.RequireCorrection();

    proposal.SubmitForReview();

    Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
    Assert.NotNull(proposal.UpdatedAtUtc);
  }

  [Fact]
  public void SubmitForReview_AfterCorrection_ShouldIncrementReviewVersion()
  {
    var proposal = new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    proposal.SubmitForReview();

    Assert.Equal(1, proposal.ReviewVersion);

    proposal.RequireCorrection();
    proposal.SubmitForReview();

    Assert.Equal(2, proposal.ReviewVersion);
    Assert.Equal(ProposalStatus.SubmittedForReview, proposal.ProposalStatus);
  }
  

    [Fact]
    public void Approve_WhenSubmittedForReview_ShouldTransitionToApproved()
    {
        var proposal = CreateSubmittedProposal();
        proposal.Approve();
        Assert.Equal(ProposalStatus.Approved, proposal.ProposalStatus);
        Assert.NotNull(proposal.UpdatedAtUtc);
    }

    [Fact]
    public void Approve_WhenNotSubmittedForReview_ShouldThrowInvalidOperationException()
    {
        var proposal = new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => proposal.Approve());
    }


    private static Proposal CreateSubmittedProposal()
    {
        var proposal = new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        proposal.SubmitForReview();
        return proposal;
    }
}
