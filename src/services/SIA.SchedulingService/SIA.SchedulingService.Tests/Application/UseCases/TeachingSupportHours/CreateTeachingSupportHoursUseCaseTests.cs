using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class CreateTeachingSupportHoursUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateTeachingSupportHours()
  {
    var tenantId = Guid.NewGuid();
    var activityId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();

    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    var dataStore = new FakeTeachingSupportHoursDataStore();
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var supportHoursQueries = new FakeTeachingSupportHoursQueries
    {
      TotalSupportHoursByAcademicLoad = 3
    };
    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(supportHoursQueries);
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new CreateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);

    var request = new CreateTeachingSupportHoursRequest
    {
      TenantId = tenantId,
      ActivityId = activityId,
      AcademicLoadId = academicLoad.Id,
      Hours = 5
    };

    var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal(activityId, response.ActivityId);
    Assert.Equal(academicLoad.Id, response.AcademicLoadId);
    Assert.Equal(5, response.Hours);
    Assert.True(response.Status);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.NotNull(dataStore.AddedTeachingSupportHours);
    Assert.Equal(5, dataStore.AddedTeachingSupportHours.Hours);
    Assert.NotNull(dataStore.AddedCreatedEvent);
    Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
    Assert.Equal(8, academicLoad.SupportHours);
    Assert.Same(academicLoad, dataStore.SavedAcademicLoad);
  }

  [Fact]
  public async Task ExecuteAsync_WhenAlreadyExists_ShouldThrowDuplicateTeachingSupportHoursException()
  {
    var tenantId = Guid.NewGuid();
    var activityId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    var academicLoad = new AcademicLoad(tenantId, proposal.Id, Guid.NewGuid(), Guid.NewGuid(), proposal.AcademicPeriodId, "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);

    var dataStore = new FakeTeachingSupportHoursDataStore
    {
      ExistsResult = true
    };
    var academicLoadDataStore = new FakeAcademicLoadDataStore(academicLoad);
    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(new FakeTeachingSupportHoursQueries());
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(proposal));
    var useCase = new CreateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);

    var request = new CreateTeachingSupportHoursRequest
    {
      TenantId = tenantId,
      ActivityId = activityId,
      AcademicLoadId = academicLoad.Id,
      Hours = 5
    };

    await Assert.ThrowsAsync<DuplicateTeachingSupportHoursException>(() =>
      useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

    Assert.Equal(0, academicLoad.SupportHours);
    Assert.Null(dataStore.SavedAcademicLoad);
    Assert.Null(dataStore.AddedTeachingSupportHours);
    Assert.Null(dataStore.AddedCreatedEvent);
  }

  [Fact]
  public async Task ExecuteAsync_WhenAcademicLoadDoesNotExist_ShouldThrowAcademicLoadNotFoundException()
  {
    var dataStore = new FakeTeachingSupportHoursDataStore();
    var academicLoadDataStore = new FakeAcademicLoadDataStore();
    var supportHoursCalculator = new AcademicLoadSupportHoursCalculator(new FakeTeachingSupportHoursQueries());
    var proposalValidator = new ProposalValidator(new FakeProposalDataStore(new Proposal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())));
    var useCase = new CreateTeachingSupportHoursUseCase(dataStore, academicLoadDataStore, supportHoursCalculator, proposalValidator);

    var request = new CreateTeachingSupportHoursRequest
    {
      TenantId = Guid.NewGuid(),
      ActivityId = Guid.NewGuid(),
      AcademicLoadId = Guid.NewGuid(),
      Hours = 5
    };

    await Assert.ThrowsAsync<AcademicLoadNotFoundException>(() =>
      useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

    Assert.Null(dataStore.SavedAcademicLoad);
    Assert.Null(dataStore.AddedTeachingSupportHours);
    Assert.Null(dataStore.AddedCreatedEvent);
  }
}
