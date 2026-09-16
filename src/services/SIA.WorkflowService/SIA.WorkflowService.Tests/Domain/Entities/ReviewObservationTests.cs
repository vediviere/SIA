using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Tests.Domain.Entities;

public sealed class ReviewObservationTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateObservation()
  {
    var processId = Guid.NewGuid();
    var tenantId = Guid.NewGuid();
    var targetId = Guid.NewGuid();
    var createdBy = Guid.NewGuid();
    var createdAtUtc = DateTime.UtcNow;
    var correlationId = Guid.NewGuid();

    var observation = new ReviewObservation(processId, tenantId, ObservationTarget.Offering, targetId, " Revisar la asignación. ", createdBy, createdAtUtc, correlationId);

    Assert.NotEqual(Guid.Empty, observation.Id);
    Assert.Equal(processId, observation.ReviewProcessId);
    Assert.Equal(tenantId, observation.TenantId);
    Assert.Equal(ObservationTarget.Offering, observation.TargetType);
    Assert.Equal(targetId, observation.TargetId);
    Assert.Equal("Revisar la asignación.", observation.Description);
    Assert.Equal(createdBy, observation.CreatedBy);
    Assert.Equal(createdAtUtc, observation.CreatedAtUtc);
    Assert.Equal(correlationId, observation.CorrelationId);
  }

  [Fact]
  public void Constructor_WithoutDescription_ShouldThrowArgumentException()
  {
    var action = () => new ReviewObservation(Guid.NewGuid(), Guid.NewGuid(), ObservationTarget.Proposal, Guid.NewGuid(), " ", Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid());

    Assert.Throws<ArgumentException>(action);
  }
}
