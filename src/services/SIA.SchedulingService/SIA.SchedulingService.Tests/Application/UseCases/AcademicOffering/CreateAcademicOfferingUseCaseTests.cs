using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Contracts.Requests.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class CreateAcademicOfferingUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateAcademicOffering()
  {
    var tenantId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var subjectId = Guid.NewGuid();
    var academicLoadId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var dataStore = new FakeAcademicOfferingDataStore();
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var offeringQueries = new FakeAcademicOfferingQueries();
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(offeringQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new CreateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    var request = new CreateAcademicOfferingRequest
    {
      TenantId = tenantId,
      GroupId = groupId,
      SubjectId = subjectId,
      AcademicLoadId = academicLoadId,
      OfferingStatus = "  ACEPTADA  "
    };
    var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal(groupId, response.GroupId);
    Assert.Equal(subjectId, response.SubjectId);
    Assert.Equal(academicLoadId, response.AcademicLoadId);
    Assert.Equal("ACEPTADA", response.OfferingStatus);
    Assert.True(response.Status);
    Assert.Equal(correlationId, response.CorrelationId);

    Assert.NotNull(dataStore.AddedAcademicOffering);
    Assert.Equal(tenantId, dataStore.AddedAcademicOffering.TenantId);
    Assert.Equal("ACEPTADA", dataStore.AddedAcademicOffering.OfferingStatus);

    Assert.NotNull(dataStore.AddedCreatedEvent);
    Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
    Assert.Equal(tenantId, dataStore.AddedCreatedEvent.TenantId);
    Assert.Equal(1, dataStore.AddedCreatedEvent.Version);
  }

  [Fact]
  public async Task ExecuteAsync_WhenOfferingAlreadyExists_ShouldThrowAcademicOfferingAlreadyExistsException()
  {
    var tenantId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var dataStore = new FakeAcademicOfferingDataStore { ExistsResult = true };
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var offeringQueries = new FakeAcademicOfferingQueries();
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(offeringQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new CreateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);
    var request = new CreateAcademicOfferingRequest
    {
      TenantId = Guid.NewGuid(),
      GroupId = Guid.NewGuid(),
      SubjectId = Guid.NewGuid(),
      AcademicLoadId = Guid.NewGuid(),
      OfferingStatus = "ACEPTADA"
    };
    await Assert.ThrowsAsync<AcademicOfferingAlreadyExistsException>(() => useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));
    Assert.Null(dataStore.AddedAcademicOffering);
    Assert.Null(dataStore.AddedCreatedEvent);
  }
}
