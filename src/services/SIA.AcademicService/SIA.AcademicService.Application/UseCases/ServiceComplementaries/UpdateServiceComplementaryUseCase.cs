using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Contracts.Requests.ServiceComplementaries;
using SIA.AcademicService.Contracts.Responses.ServiceComplementaries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.UseCases.ServiceComplementaries;

public sealed class UpdateServiceComplementaryUseCase
{
    private readonly IServiceComplementaryDataStore _dataStore;

    public UpdateServiceComplementaryUseCase(IServiceComplementaryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateServiceComplementaryResponse> ExecuteAsync(
        Guid tenantId,
        Guid serviceComplementaryId,
        UpdateServiceComplementaryRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var serviceComplementary = await _dataStore.GetServiceComplementaryByIdAsync(
            tenantId,
            serviceComplementaryId,
            cancellationToken);

        if (serviceComplementary is null)
        {
            throw new ServiceComplementaryNotFoundException(serviceComplementaryId);
        }

        serviceComplementary.Update(request.Type, request.Credit);

        var integrationEvent = new ServiceComplementaryUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = serviceComplementary.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = serviceComplementary.TenantId,
            ServiceComplementaryId = serviceComplementary.Id,
            Type = serviceComplementary.Type,
            Credit = serviceComplementary.Credit,
            Status = serviceComplementary.Status,
            Version = 1
        };

        await _dataStore.UpdateServiceComplementaryWithOutboxAsync(serviceComplementary, integrationEvent, cancellationToken);

        return new UpdateServiceComplementaryResponse
        {
            Id = serviceComplementary.Id,
            Type = serviceComplementary.Type,
            Credit = serviceComplementary.Credit,
            UpdatedAtUtc = serviceComplementary.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}