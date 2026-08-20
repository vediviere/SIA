using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeAcademicLoadDataStore : IAcademicLoadDataStore
{
    private readonly AcademicLoad? _academicLoaoReturn;

    public FakeAcademicLoadDataStore(AcademicLoad? academicLoadReturn = null)
    {
        _academicLoaoReturn = academicLoadReturn;
    }
    public bool AcademicLoadAdded { get; private set; }
    public bool AcademicLoadUpdated { get; private set; }
    public bool AcademicLoadActivated { get; private set; }
    public bool AcademicLoadDeactivated { get; private set; }

    public Task<AcademicLoad?> GetByIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken) => Task.FromResult(_academicLoaoReturn);
    public Task AddAcademicLoadWithOutboxAsync(AcademicLoad academicLoad, AcademicLoadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AcademicLoadAdded = true;
        return Task.CompletedTask;
    }
    public Task UpdateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AcademicLoadUpdated = true;
        return Task.CompletedTask;
    }
    public Task ActivateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AcademicLoadActivated = true;
        return Task.CompletedTask;
    }
    public Task DeactivateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AcademicLoadDeactivated = true;
        return Task.CompletedTask;
    }
}