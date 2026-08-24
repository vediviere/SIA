using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeCoordinatorDataStore : ICoordinatorDataStore
{
    public Coordinator? CoordinatorById { get; set; }
    public bool PersonAlreadyCoordinatorResult { get; set; }

    public Coordinator? AddedCoordinator { get; private set; }
    public CoordinatorCreatedIntegrationEvent? AddedEvent { get; private set; }

    public Coordinator? ActivatedCoordinator { get; private set; }
    public CoordinatorActivatedIntegrationEvent? ActivatedEvent { get; private set; }

    public Coordinator? DeactivatedCoordinator { get; private set; }
    public CoordinatorDeactivatedIntegrationEvent? DeactivatedEvent { get; private set; }


    public Task<bool> PersonAlreadyCoordinatorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonAlreadyCoordinatorResult);

    public Task<Coordinator?> GetCoordinatorByIdAsync(Guid tenantId, Guid coordinatorId, CancellationToken cancellationToken)
        => Task.FromResult(CoordinatorById);

    public Task AddCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedCoordinator = coordinator;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ActivateCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivatedCoordinator = coordinator;
        ActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task DeactivateCoordinatorWithOutboxAsync(Coordinator coordinator, CoordinatorDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeactivatedCoordinator = coordinator;
        DeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}