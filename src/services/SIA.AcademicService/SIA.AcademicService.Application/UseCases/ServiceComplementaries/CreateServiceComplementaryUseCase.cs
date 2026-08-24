using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Contracts.Requests.ServiceComplementaries;
using SIA.AcademicService.Contracts.Responses.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.UseCases.ServiceComplementaries;

public sealed class CreateServiceComplementaryUseCase
{
    private readonly IServiceComplementaryDataStore _dataStore;

    public CreateServiceComplementaryUseCase(IServiceComplementaryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateServiceComplementaryResponse> ExecuteAsync(
        CreateServiceComplementaryRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var serviceComplementary = new ServiceComplementary(
            request.TenantId,
            request.StudyPlanId,
            request.Type,
            request.Credit);

        var integrationEvent = new ServiceComplementaryCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = serviceComplementary.CreatedAtUtc,
            TenantId = serviceComplementary.TenantId,
            ServiceComplementaryId = serviceComplementary.Id,
            StudyPlanId = serviceComplementary.StudyPlanId,
            Type = serviceComplementary.Type,
            Credit = serviceComplementary.Credit,
            Status = serviceComplementary.Status,
            Version = 1
        };

        await _dataStore.AddServiceComplementaryWithOutboxAsync(serviceComplementary, integrationEvent, cancellationToken);

        return new CreateServiceComplementaryResponse
        {
            Id = serviceComplementary.Id,
            TenantId = serviceComplementary.TenantId,
            StudyPlanId = serviceComplementary.StudyPlanId,
            Type = serviceComplementary.Type,
            Credit = serviceComplementary.Credit,
            Status = serviceComplementary.Status,
            CreatedAtUtc = serviceComplementary.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}