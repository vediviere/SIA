using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeAcademicLoadDataStore : IAcademicLoadDataStore
{
    private readonly AcademicLoad? _academicLoadReturn;

    public FakeAcademicLoadDataStore(AcademicLoad? academicLoadReturn = null)
    {
        _academicLoadReturn = academicLoadReturn;
    }
    public AcademicLoad? AddedAcademicLoad { get; private set; }
    public AcademicLoad? UpdatedAcademicLoad { get; private set; }
    public AcademicLoadCreatedIntegrationEvent? AddedCreatedEvent { get; private set; }
    public AcademicLoadUpdatedIntegrationEvent? AddedUpdatedEvent { get; private set; }
    public AcademicLoadActivatedIntegrationEvent? AddedActivatedEvent { get; private set; }
    public AcademicLoadDeactivatedIntegrationEvent? AddedDeactivatedEvent { get; private set; }

    public Task<AcademicLoad?> GetByIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken) => Task.FromResult(_academicLoadReturn);
    public Task AddAcademicLoadWithOutboxAsync(AcademicLoad academicLoad, AcademicLoadCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedAcademicLoad = academicLoad;
        AddedCreatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task UpdateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedAcademicLoad = academic;
        AddedUpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task ActivateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task DeactivateAcademicLoadWithOutboxAsync(AcademicLoad academic, AcademicLoadDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedDeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}