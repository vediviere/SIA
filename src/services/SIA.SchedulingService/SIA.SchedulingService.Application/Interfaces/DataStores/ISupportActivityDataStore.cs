using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface ISupportActivityDataStore
{
    Task<SupportActivity?> GetSupportActivityByIdAsync(Guid tenantId, Guid supportActivityId, CancellationToken cancellationToken);

    Task AddSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task UpdateSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task SoftDeleteSupportActivityWithOutboxAsync(SupportActivity supportActivity,SupportActivityDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task RestoreSupportActivityWithOutboxAsync(SupportActivity supportActivity, SupportActivityRestoredIntegrationEvent integrationEvent,CancellationToken cancellationToken);
}