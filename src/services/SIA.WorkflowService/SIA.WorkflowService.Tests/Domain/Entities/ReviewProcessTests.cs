using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Tests.Domain.Entities;

public sealed class ReviewProcessTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateProcessInReview()
  {
    var tenantId = Guid.NewGuid();
    var proposalId = Guid.NewGuid();
    var divisionHeadId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var submittedAtUtc = DateTime.UtcNow;

    var process = new ReviewProcess(tenantId, proposalId, divisionHeadId, 1, submittedAtUtc, correlationId);

    Assert.NotEqual(Guid.Empty, process.Id);
    Assert.Equal(tenantId, process.TenantId);
    Assert.Equal(proposalId, process.AcademicLoadProposalId);
    Assert.Equal(divisionHeadId, process.DivisionHeadId);
    Assert.Equal(1, process.Version);
    Assert.Equal(ReviewStatus.InReview, process.Status);
    Assert.Equal(submittedAtUtc, process.SubmittedAtUtc);
    Assert.Equal(correlationId, process.CorrelationId);
    Assert.NotEqual(default, process.CreatedAtUtc);
    Assert.Null(process.UpdatedAtUtc);
  }

  [Fact]
  public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
  {
    var action = () => new ReviewProcess(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());

    Assert.Throws<ArgumentException>(action);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Constructor_WithInvalidVersion_ShouldThrowArgumentOutOfRangeException(int version)
  {
    var action = () => new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), version, DateTime.UtcNow, Guid.NewGuid());

    Assert.Throws<ArgumentOutOfRangeException>(action);
  }

  [Fact]
  public void Approve_WithValidData_ShouldCompleteProcess()
  {
    var process = new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());
    var decidedBy = Guid.NewGuid();
    var decidedAtUtc = DateTime.UtcNow;
    var correlationId = Guid.NewGuid();

    process.Approve(decidedBy, decidedAtUtc, correlationId);

    Assert.Equal(ReviewStatus.Approved, process.Status);
    Assert.Equal(decidedBy, process.DecidedBy);
    Assert.Equal(decidedAtUtc, process.DecidedAtUtc);
    Assert.Equal(correlationId, process.DecisionCorrelationId);
    Assert.Equal(decidedAtUtc, process.UpdatedAtUtc);
  }

  [Fact]
  public void Approve_WhenProcessIsFinalized_ShouldThrowInvalidOperationException()
  {
    var process = new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());

    process.Approve(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid());

    var action = () => process.Approve(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid());

    Assert.Throws<InvalidOperationException>(action);
  }

  [Fact]
  public void ReturnForCorrection_WithObservation_ShouldCompleteProcess()
  {
    var process = new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());
    var decidedBy = Guid.NewGuid();
    var decidedAtUtc = DateTime.UtcNow;
    var correlationId = Guid.NewGuid();
    var observation = new ReviewObservation(process.Id, process.TenantId, ObservationTarget.Offering, Guid.NewGuid(), "Revisar las horas asignadas.", decidedBy, decidedAtUtc, correlationId);

    process.ReturnForCorrection(decidedBy, decidedAtUtc, correlationId, [observation]);

    Assert.Equal(ReviewStatus.RequiresCorrection, process.Status);
    Assert.Equal(decidedBy, process.DecidedBy);
    Assert.Equal(decidedAtUtc, process.DecidedAtUtc);
    Assert.Equal(correlationId, process.DecisionCorrelationId);
    Assert.Same(observation, Assert.Single(process.Observations));
  }

  [Fact]
  public void ReturnForCorrection_WithoutObservations_ShouldThrowArgumentException()
  {
    var process = new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());

    var action = () => process.ReturnForCorrection(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), []);

    Assert.Throws<ArgumentException>(action);
    Assert.Equal(ReviewStatus.InReview, process.Status);
  }
}
