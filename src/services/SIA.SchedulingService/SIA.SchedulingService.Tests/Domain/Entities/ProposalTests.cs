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
}
