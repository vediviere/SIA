using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.UseCases.ServiceComplementaries;

public sealed class RestoreServiceComplementaryUseCase
{
    private readonly IServiceComplementaryDataStore _dataStore;

    public RestoreServiceComplementaryUseCase(IServiceComplementaryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid serviceComplementaryId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var serviceComplementary = await _dataStore.GetServiceComplementaryByIdAsync(tenantId, serviceComplementaryId, cancellationToken);

        if (serviceComplementary is null)
        {
            throw new ServiceComplementaryNotFoundException(serviceComplementaryId);
        }

        serviceComplementary.Restore();

        var integrationEvent = new ServiceComplementaryRestoredIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = serviceComplementary.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = serviceComplementary.TenantId,
            ServiceComplementaryId = serviceComplementary.Id,
            Version = 1
        };

        await _dataStore.RestoreServiceComplementaryWithOutboxAsync(serviceComplementary, integrationEvent, cancellationToken);
    }
}