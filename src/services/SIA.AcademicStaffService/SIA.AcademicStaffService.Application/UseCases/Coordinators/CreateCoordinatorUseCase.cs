using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Contracts.Requests.Coordinators;
using SIA.AcademicStaffService.Contracts.Responses.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.Coordinators;

public sealed class CreateCoordinatorUseCase
{
    private readonly ICoordinatorDataStore _dataStore;

    public CreateCoordinatorUseCase(ICoordinatorDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateCoordinatorResponse> ExecuteAsync(
        CreateCoordinatorRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var personAlreadyCoordinator = await _dataStore.PersonAlreadyCoordinatorAsync(
            request.TenantId,
            request.PersonId,
            cancellationToken);

        if (personAlreadyCoordinator)
        {
            throw new DuplicateCoordinatorException(request.PersonId);
        }

        var coordinator = new Coordinator(
            request.TenantId,
            request.PersonId,
            request.AcademicDegree);

        var integrationEvent = new CoordinatorCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = coordinator.CreatedAtUtc,
            TenantId = coordinator.TenantId,
            CoordinatorId = coordinator.Id,
            PersonId = coordinator.PersonId,
            AcademicDegree = coordinator.AcademicDegree,
            Status = coordinator.Status,
            Version = 1
        };

        await _dataStore.AddCoordinatorWithOutboxAsync(coordinator, integrationEvent, cancellationToken);

        return new CreateCoordinatorResponse
        {
            Id = coordinator.Id,
            TenantId = coordinator.TenantId,
            PersonId = coordinator.PersonId,
            AcademicDegree = coordinator.AcademicDegree,
            Status = coordinator.Status,
            CreatedAtUtc = coordinator.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}