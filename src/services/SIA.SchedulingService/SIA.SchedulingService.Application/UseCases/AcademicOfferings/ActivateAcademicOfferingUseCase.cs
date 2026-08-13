using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;

namespace SIA.SchedulingService.Application.UseCases.AcademicOfferings;

public sealed class ActivateAcademicOfferingUseCase
{
    private readonly IAcademicOfferingDataStore _dataStore;

    public ActivateAcademicOfferingUseCase(IAcademicOfferingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
    {
        var academicOffering = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if (academicOffering is null)
        {
            throw new AcademicOfferingNotFoundException(id);
        }

        academicOffering.Activate();

        var integrationEvent = new AcademicOfferingActivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = academicOffering.UpdatedAtUtc!.Value,
            TenantId = academicOffering.TenantId,
            OfferingId = academicOffering.Id,
            Status = academicOffering.Status,
            Version = 1
        };

        await _dataStore.ActivateAcademicOfferingWithOutboxAsync(academicOffering, integrationEvent, cancellationToken);
    }
}