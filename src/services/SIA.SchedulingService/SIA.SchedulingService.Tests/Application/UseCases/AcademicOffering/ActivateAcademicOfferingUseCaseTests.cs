using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;


namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class ActivateAcademicOfferingUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidOffering_ShouldActivateAndPublishEvent()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var offering = new AcademicOffering(tenantId, Guid.NewGuid(), Guid.NewGuid(), academicLoad.Id, "OFERTADA");
    offering.AssignClassHours(4);
    offering.Deactivate();

    var dataStore = new FakeAcademicOfferingDataStore(offering);
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var offeringQueries = new FakeAcademicOfferingQueries
    {
      TotalClassHoursByAcademicLoad = 6
    };
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(offeringQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new ActivateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    await useCase.ExecuteAsync(tenantId, offering.Id, correlationId, CancellationToken.None);

    Assert.True(offering.Status);
    Assert.NotNull(offering.UpdatedAtUtc);
    Assert.NotNull(dataStore.AddedActivatedEvent);
    Assert.Equal(correlationId, dataStore.AddedActivatedEvent.CorrelationId);
    Assert.True(dataStore.AddedActivatedEvent.Status);
    Assert.Equal(10, academicLoad.ClassHours);
    Assert.Same(academicLoad, dataStore.SavedAcademicLoad);
  }

  [Fact]
  public async Task ExecuteAsync_WhenOfferingDoesNotExist_ShouldThrowAcademicOfferingNotFoundException()
  {
    var dataStore = new FakeAcademicOfferingDataStore(null);
    var academicLoadDataStore = new FakeAcademicLoadDataStore();
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(new FakeAcademicOfferingQueries());
var proposalValidator = new ProposalValidator(new FakeProposalDataStore(new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())));
    var useCase = new ActivateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    await Assert.ThrowsAsync<AcademicOfferingNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.AddedActivatedEvent);
    Assert.Null(dataStore.SavedAcademicLoad);
  }
}
