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

    public Task<bool> ExistsByGroupAndSubjectAsync(Guid groupId, Guid subjectId, CancellationToken cancellationToken) => Task.FromResult(ExistsResult);
    public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken) => Task.FromResult(_offeringReturn);
    public Task AddAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingCreatedIntegrationEvet integrationEvent, CancellationToken cancellationToken)
    {
        AddedAcademicOffering = academicOffering;
        AddedCreatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task UpdateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedAcademicOffering = academicOffering;
        AddedUpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task ActivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task DeactivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedDeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}