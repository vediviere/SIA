using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.DataStores;

public interface IServiceComplementaryDataStore
{
    Task<ServiceComplementary?> GetServiceComplementaryByIdAsync(Guid tenantId, Guid serviceComplementaryId, CancellationToken cancellationToken);

    Task AddServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task UpdateServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task SoftDeleteServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}