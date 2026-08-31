using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoads;

public sealed class CreateAcademicLoadUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateAcademicLoad()
  {
    var tenantId = Guid.NewGuid();
    var academicPeriodId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), academicPeriodId, Guid.NewGuid());

    var dataStore = new FakeAcademicLoadDataStore();
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new CreateAcademicLoadUseCase(dataStore, proposalValidator);
    var request = new CreateAcademicLoadRequest
    {
      TenantId = tenantId,
      ProposalId = proposal.Id,
      TeacherId = Guid.NewGuid(),
      DivisionId = Guid.NewGuid(),
      AcademicPeriodId = academicPeriodId,
      OfficialLetterNumber = "  OF-2026-001  ",
      ProposedDate = DateTime.UtcNow,
      AssignmentDate = DateTime.UtcNow
    };

    var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal(proposal.Id, response.ProposalId);
    Assert.Equal(academicPeriodId, response.AcademicPeriodId);
    Assert.Equal("OF-2026-001", response.OfficialLetterNumber);
    Assert.Equal(0, response.ClassHours);
    Assert.Equal(0, response.SupportHours);
    Assert.True(response.Status);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.NotNull(dataStore.AddedAcademicLoad);
    Assert.Equal(proposal.Id, dataStore.AddedAcademicLoad.ProposalId);
    Assert.NotNull(dataStore.AddedCreatedEvent);
    Assert.Equal(proposal.Id, dataStore.AddedCreatedEvent.ProposalId);
    Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenProposalIsNotEditable_ShouldThrowProposalNotEditableException()
  {
    var dataStore = new FakeAcademicLoadDataStore();
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore());
    var useCase = new CreateAcademicLoadUseCase(dataStore, proposalValidator);
    var request = new CreateAcademicLoadRequest
    {
      TenantId = Guid.NewGuid(),
      ProposalId = Guid.NewGuid(),
      TeacherId = Guid.NewGuid(),
      DivisionId = Guid.NewGuid(),
      AcademicPeriodId = Guid.NewGuid(),
      OfficialLetterNumber = "OF-2026-001",
      ProposedDate = DateTime.UtcNow,
      AssignmentDate = DateTime.UtcNow
    };

    await Assert.ThrowsAsync<ProposalNotEditableException>(() =>
      useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.AddedAcademicLoad);
    Assert.Null(dataStore.AddedCreatedEvent);
  }
}
