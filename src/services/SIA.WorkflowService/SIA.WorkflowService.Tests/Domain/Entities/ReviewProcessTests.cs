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
}
