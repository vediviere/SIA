using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Contracts.Enums;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadProposal;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicLoadProposals;

public sealed class CreateProposalUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateDraftProposal()
  {
    var tenantId = Guid.NewGuid();
    var educationalProgramId = Guid.NewGuid();
    var academicPeriodId = Guid.NewGuid();
    var divisionHeadId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var dataStore = new FakeProposalDataStore();
    var useCase = new CreateProposalUseCase(dataStore);
    var request = new CreateProposalRequest
    {
      TenantId = tenantId,
      EducationalProgramId = educationalProgramId,
      AcademicPeriodId = academicPeriodId,
      DivisionHeadId = divisionHeadId
    };

    var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

    Assert.NotEqual(Guid.Empty, response.Id);
    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal(educationalProgramId, response.EducationalProgramId);
    Assert.Equal(academicPeriodId, response.AcademicPeriodId);
    Assert.Equal(divisionHeadId, response.DivisionHeadId);
    Assert.Equal(ProposalStatus.Draft, response.ProposalStatus);
    Assert.True(response.Status);
    Assert.Equal(correlationId, response.CorrelationId);

    Assert.NotNull(dataStore.AddedProposal);
    Assert.Equal(response.Id, dataStore.AddedProposal.Id);
    Assert.NotNull(dataStore.AddedCreatedEvent);
    Assert.Equal(response.Id, dataStore.AddedCreatedEvent.ProposalId);
    Assert.Equal(ProposalStatus.Draft, dataStore.AddedCreatedEvent.ProposalStatus);
    Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenProposalExists_ShouldThrowProposalAlreadyExistsException()
  {
    var dataStore = new FakeProposalDataStore
    {
      ExistsResult = true
    };
    var useCase = new CreateProposalUseCase(dataStore);
    var request = new CreateProposalRequest
    {
      TenantId = Guid.NewGuid(),
      EducationalProgramId = Guid.NewGuid(),
      AcademicPeriodId = Guid.NewGuid(),
      DivisionHeadId = Guid.NewGuid()
    };

    await Assert.ThrowsAsync<ProposalAlreadyExistsException>(() =>
      useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.AddedProposal);
    Assert.Null(dataStore.AddedCreatedEvent);
  }
}
