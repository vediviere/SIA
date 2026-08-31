using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Contracts.Requests;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class UpdateAcademicOfferingUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldUpdateAcademicOffering()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var offering = new AcademicOffering(tenantId, Guid.NewGuid(), Guid.NewGuid(), academicLoad.Id, "NO ACEPTADA");
    offering.AssignClassHours(4);

    var dataStore = new FakeAcademicOfferingDataStore(offering);
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var offeringQueries = new FakeAcademicOfferingQueries
    {
      TotalClassHoursByAcademicLoad = 6
    };
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(offeringQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new UpdateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    var request = new UpdateAcademicOfferingRequest
    {
      OfferingStatus = "  ACEPTADA  "
    };

    var response = await useCase.ExecuteAsync(tenantId, offering.Id, request, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal("ACEPTADA", response.OfferingStatus);
    Assert.NotNull(response.UpdatedAtUtc);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.NotNull(dataStore.UpdatedAcademicOffering);
    Assert.Equal("ACEPTADA", dataStore.UpdatedAcademicOffering.OfferingStatus);
    Assert.NotNull(dataStore.AddedUpdatedEvent);
    Assert.Equal(correlationId, dataStore.AddedUpdatedEvent.CorrelationId);
    Assert.Equal(10, academicLoad.ClassHours);
    Assert.Same(academicLoad, dataStore.SavedAcademicLoad);
  }

  [Fact]
  public async Task ExecuteAsync_WhenOfferingDoesNotExist_ShouldThrowAcademicOfferingNotFoundException()
  {
    var dataStore = new FakeAcademicOfferingDataStore(null);
    var academicLoadDataStore = new FakeAcademicLoadDataStore();
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(new FakeAcademicOfferingQueries());
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore());
    var useCase = new UpdateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    var request = new UpdateAcademicOfferingRequest
    {
      OfferingStatus = "ACEPTADA"
    };

    await Assert.ThrowsAsync<AcademicOfferingNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.UpdatedAcademicOffering);
    Assert.Null(dataStore.AddedUpdatedEvent);
    Assert.Null(dataStore.SavedAcademicLoad);
  }
}
