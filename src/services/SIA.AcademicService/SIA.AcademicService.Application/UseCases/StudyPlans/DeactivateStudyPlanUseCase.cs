using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;

namespace SIA.AcademicService.Application.UseCases.StudyPlans;
public sealed class DeactivateStudyPlanUseCase
{
    private readonly IStudyPlanDataStore _dataStore;

    public DeactivateStudyPlanUseCase(IStudyPlanDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);
        if (entity is null)
        {
            throw new StudyPlanNotFoundException(id);
        }

        entity.Deactivate();

        var integrationEvent = new StudyPlanDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = entity.TenantId,
            StudyPlanId = entity.Id,
            ContractVersion = 1
        };

        await _dataStore.DeactivateStudyPlanWithOutboxAsync(entity, integrationEvent, cancellationToken);
    }
}