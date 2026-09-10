using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.AcademicOfferings;

public sealed class DeactivateAcademicOfferingUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidOffering_ShouldDeactivateAndPublishEvent()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var offering = new AcademicOffering(tenantId, Guid.NewGuid(), Guid.NewGuid(), academicLoad.Id, "OFERTADA");
    offering.AssignClassHours(4);

    var dataStore = new FakeAcademicOfferingDataStore(offering);
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var offeringQueries = new FakeAcademicOfferingQueries
    {
      TotalClassHoursByAcademicLoad = 6
    };
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(offeringQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new DeactivateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    await useCase.ExecuteAsync(tenantId, offering.Id, correlationId, CancellationToken.None);

    Assert.False(offering.Status);
    Assert.NotNull(offering.UpdatedAtUtc);
    Assert.NotNull(dataStore.AddedDeactivatedEvent);
    Assert.Equal(correlationId, dataStore.AddedDeactivatedEvent.CorrelationId);
    Assert.False(dataStore.AddedDeactivatedEvent.Status);
    Assert.Equal(6, academicLoad.ClassHours);
    Assert.Same(academicLoad, dataStore.SavedAcademicLoad);
  }

  [Fact]
  public async Task ExecuteAsync_WhenOfferingDoesNotExist_ShouldThrowAcademicOfferingNotFoundException()
  {
    var dataStore = new FakeAcademicOfferingDataStore(null);
    var academicLoadDataStore = new FakeAcademicLoadDataStore();
    var classHoursCalculator = new AcademicLoadClassHoursCalculator(new FakeAcademicOfferingQueries());
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore());
    var useCase = new DeactivateAcademicOfferingUseCase(dataStore, academicLoadDataStore, classHoursCalculator, proposalValidator);

    await Assert.ThrowsAsync<AcademicOfferingNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.AddedDeactivatedEvent);
    Assert.Null(dataStore.SavedAcademicLoad);
  }
}
