using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class ActivateTeachingSupportHoursUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidSupportHours_ShouldActivateAndPublishEvent()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var tsh = new TeachingSupportHour(tenantId, Guid.NewGuid(), academicLoad.Id, 5);
    tsh.Deactivate();

    var dataStore = new FakeTeachingSupportHoursDataStore(tsh);
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var supportHoursQueries = new FakeTeachingSupportHoursQueries
    {
      TotalSupportHoursByAcademicLoad = 3
    };
    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(supportHoursQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new ActivateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);

    await useCase.ExecuteAsync(tenantId, tsh.Id, correlationId, CancellationToken.None);

    Assert.True(tsh.Status);
    Assert.NotNull(tsh.UpdatedAtUtc);
    Assert.NotNull(dataStore.AddedActivatedEvent);
    Assert.Equal(correlationId, dataStore.AddedActivatedEvent.CorrelationId);
    Assert.True(dataStore.AddedActivatedEvent.Status);
    Assert.Equal(8, academicLoad.SupportHours);
    Assert.Same(academicLoad, dataStore.SavedAcademicLoad);
  }

  [Fact]
  public async Task ExecuteAsync_WhenDoesNotExist_ShouldThrowTeachingSupportHoursNotFoundException()
  {
    var dataStore = new FakeTeachingSupportHoursDataStore(null);
    var academicLoadDataStore = new FakeAcademicLoadDataStore();
    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(new FakeTeachingSupportHoursQueries());
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore());
    var useCase = new ActivateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);

    await Assert.ThrowsAsync<TeachingSupportHoursNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.AddedActivatedEvent);
    Assert.Null(dataStore.SavedAcademicLoad);
  }
}
