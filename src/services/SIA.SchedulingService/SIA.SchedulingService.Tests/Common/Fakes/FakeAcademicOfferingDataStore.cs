using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeAcademicOfferingDataStore : IAcademicOfferingDataStore
{
  private readonly AcademicOffering? _offeringReturn;

  public FakeAcademicOfferingDataStore(AcademicOffering? offeringReturn = null)
  {
    _offeringReturn = offeringReturn;
  }
  public bool ExistsResult { get; set; }
  public AcademicOffering? AddedAcademicOffering { get; private set; }
  public AcademicOffering? UpdatedAcademicOffering { get; private set; }
  public AcademicOfferingCreatedIntegrationEvet? AddedCreatedEvent { get; private set; }
  public AcademicOfferingUpdatedIntegrationEvent? AddedUpdatedEvent { get; private set; }
  public AcademicOfferingActivatedIntegrationEvent? AddedActivatedEvent { get; private set; }
  public AcademicOfferingDeactivatedIntegrationEvent? AddedDeactivatedEvent { get; private set; }
  public AcademicLoad? SavedAcademicLoad { get; private set; }
  public int TotalClassHoursByAcademicLoad { get; set; }

  public Task<bool> ExistsByGroupAndSubjectAsync(Guid tenantId, Guid groupId, Guid subjectId, CancellationToken cancellationToken) => Task.FromResult(ExistsResult);
  public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken) => Task.FromResult(_offeringReturn);
  public Task AddAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingCreatedIntegrationEvet integrationEvent, CancellationToken cancellationToken)
  {
    AddedAcademicOffering = academicOffering;
    AddedCreatedEvent = integrationEvent;
    SavedAcademicLoad = academicLoad;
    return Task.CompletedTask;
  }
  public Task UpdateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    UpdatedAcademicOffering = academicOffering;
    SavedAcademicLoad = academicLoad;
    AddedUpdatedEvent = integrationEvent;
    return Task.CompletedTask;
  }
  public Task ActivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    AddedActivatedEvent = integrationEvent;
    SavedAcademicLoad = academicLoad;
    return Task.CompletedTask;
  }
  public Task DeactivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    AddedDeactivatedEvent = integrationEvent;
    SavedAcademicLoad = academicLoad;
    return Task.CompletedTask;
  }
  public Task<int> GetTotalClassHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedOfferingId, CancellationToken cancellationToken)
  {
    return Task.FromResult(TotalClassHoursByAcademicLoad);
  }
}
