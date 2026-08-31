using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoads;

public sealed class UpdateAcademicLoadUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldUpdateAcademicLoad()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var existingLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-OLD-100", DateTime.UtcNow, 15, 5, DateTime.UtcNow);

    var dataStore = new FakeAcademicLoadDataStore(existingLoad);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new UpdateAcademicLoadUseCase(dataStore, proposalValidator);
    var request = new UpdateAcademicLoadRequest
    {
      OfficialLetterNumber = "  OF-NEW-200  ",
      ProposedDate = DateTime.UtcNow,
      AssignmentDate = DateTime.UtcNow
    };

    var response = await useCase.ExecuteAsync(tenantId, existingLoad.Id, request, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal(proposal.Id, response.ProposalId);
    Assert.Equal("OF-NEW-200", response.OfficialLetterNumber);
    Assert.Equal(15, response.ClassHours);
    Assert.Equal(5, response.SupportHours);
    Assert.NotNull(response.UpdatedAtUtc);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.NotNull(dataStore.UpdatedAcademicLoad);
    Assert.Equal("OF-NEW-200", dataStore.UpdatedAcademicLoad.OfficialLetterNumber);
    Assert.NotNull(dataStore.AddedUpdatedEvent);
    Assert.Equal(proposal.Id, dataStore.AddedUpdatedEvent.ProposalId);
    Assert.Equal(correlationId, dataStore.AddedUpdatedEvent.CorrelationId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenAcademicLoadDoesNotExist_ShouldThrowAcademicLoadNotFoundException()
  {
    var dataStore = new FakeAcademicLoadDataStore();
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore());
    var useCase = new UpdateAcademicLoadUseCase(dataStore, proposalValidator);
    var request = new UpdateAcademicLoadRequest
    {
      OfficialLetterNumber = "OF-NEW-200",
      ProposedDate = DateTime.UtcNow,
      AssignmentDate = DateTime.UtcNow
    };

    await Assert.ThrowsAsync<AcademicLoadNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.UpdatedAcademicLoad);
    Assert.Null(dataStore.AddedUpdatedEvent);
  }
}
