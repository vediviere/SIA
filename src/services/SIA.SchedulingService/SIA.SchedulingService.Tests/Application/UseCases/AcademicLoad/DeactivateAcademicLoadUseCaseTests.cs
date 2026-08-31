using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoads;

public sealed class DeactivateAcademicLoadUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidAcademicLoad_ShouldDeactivateAndPublishEvent()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 20, 10, DateTime.UtcNow);

    var dataStore = new FakeAcademicLoadDataStore(academicLoad);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new DeactivateAcademicLoadUseCase(dataStore, proposalValidator);

    await useCase.ExecuteAsync(tenantId, academicLoad.Id, correlationId, CancellationToken.None);

    Assert.False(academicLoad.Status);
    Assert.NotNull(academicLoad.UpdatedAtUtc);
    Assert.NotNull(dataStore.AddedDeactivatedEvent);
    Assert.Equal(correlationId, dataStore.AddedDeactivatedEvent.CorrelationId);
    Assert.False(dataStore.AddedDeactivatedEvent.Status);
    Assert.Equal(proposal.Id, dataStore.AddedDeactivatedEvent.ProposalId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenAcademicLoadDoesNotExist_ShouldThrowAcademicLoadNotFoundException()
  {
    var dataStore = new FakeAcademicLoadDataStore();
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore());
    var useCase = new DeactivateAcademicLoadUseCase(dataStore, proposalValidator);

    await Assert.ThrowsAsync<AcademicLoadNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.AddedDeactivatedEvent);
  }
}
