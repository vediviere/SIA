using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class UpdateTeachingSupportHoursUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldUpdateTeachingSupportHours()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var existingTSH = new TeachingSupportHour(tenantId, Guid.NewGuid(), academicLoad.Id, 5);

    var dataStore = new FakeTeachingSupportHoursDataStore(existingTSH);
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var supportHoursQueries = new FakeTeachingSupportHoursQueries
    {
      TotalSupportHoursByAcademicLoad = 3
    };
    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(supportHoursQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new UpdateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);

    var request = new UpdateTeachingSupportHoursRequest
    {
      Hours = 10
    };

    var response = await useCase.ExecuteAsync(tenantId, existingTSH.Id, request, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal(10, response.Hours);
    Assert.NotNull(response.UpdatedAtUtc);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.NotNull(dataStore.UpdatedTeachingSupportHours);
    Assert.Equal(10, dataStore.UpdatedTeachingSupportHours.Hours);
    Assert.NotNull(dataStore.AddedUpdatedEvent);
    Assert.Equal(correlationId, dataStore.AddedUpdatedEvent.CorrelationId);
    Assert.Equal(13, academicLoad.SupportHours);
    Assert.Same(academicLoad, dataStore.SavedAcademicLoad);
  }

  [Fact]
  public async Task ExecuteAsync_WhenDoesNotExist_ShouldThrowTeachingSupportHoursNotFoundException()
  {
    var dataStore = new FakeTeachingSupportHoursDataStore(null);
    var academicLoadDataStore = new FakeAcademicLoadDataStore();

    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(new FakeTeachingSupportHoursQueries());
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())));
    var useCase = new UpdateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);
    var request = new UpdateTeachingSupportHoursRequest { Hours = 10 };

    await Assert.ThrowsAsync<TeachingSupportHoursNotFoundException>(() =>
      useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), request, Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.UpdatedTeachingSupportHours);
    Assert.Null(dataStore.AddedUpdatedEvent);
    Assert.Null(dataStore.SavedAcademicLoad);
  }
}
