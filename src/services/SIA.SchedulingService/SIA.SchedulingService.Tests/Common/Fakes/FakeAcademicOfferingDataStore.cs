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
    public bool OfferingAdded { get; private set; }
    public bool OfferingUpdated { get; private set; }
    public bool OfferingActivated { get; private set; }
    public bool OfferingDeactivated { get; private set; }

    public Task<bool> ExistsByGroupAndSubjectAsync(Guid groupId, Guid subjectId, CancellationToken cancellationToken) => Task.FromResult(ExistsResult);
    public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken) => Task.FromResult(_offeringReturn);
    public Task AddAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingCreatedIntegrationEvet integrationEvent, CancellationToken cancellationToken)
    {
        OfferingAdded = true;
        return Task.CompletedTask;
    }
    public Task UpdateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        OfferingUpdated = true;
        return Task.CompletedTask;
    }
    public Task ActivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        OfferingActivated = true;
        return Task.CompletedTask;
    }
    public Task DeactivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicOfferingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        OfferingDeactivated = true;
        return Task.CompletedTask;
    }
}