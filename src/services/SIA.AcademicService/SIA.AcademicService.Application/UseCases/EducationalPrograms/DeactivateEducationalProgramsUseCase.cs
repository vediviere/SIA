using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;

namespace SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;

public sealed class DeactivateEducationalProgramsUseCase
{
    private readonly IEducationalProgramDataStore _dataStore;

    public DeactivateEducationalProgramsUseCase(IEducationalProgramDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if (entity is null)
        {
            throw new EducationalProgramNotFoundException(id);
        }

        entity.Desactivate();

        var integrationEvent = new EducationalProgramDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = entity.TenantId,
            EducationalProgramId = entity.Id,
            Version = 1
        };

        await _dataStore.DeactivateEducationalProgramWithOutboxAsync(entity, integrationEvent, cancellationToken);
    }
}