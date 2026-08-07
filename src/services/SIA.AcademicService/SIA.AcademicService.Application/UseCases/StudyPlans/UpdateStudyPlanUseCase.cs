using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Contracts.Requests.StudyPlans;
using SIA.AcademicService.Contracts.Responses.StudyPlans;

namespace SIA.AcademicService.Application.UseCases.StudyPlans;

public sealed class UpdateStudyPlanUseCase
{
    private readonly IStudyPlanDataStore _dataStore;

    public UpdateStudyPlanUseCase(IStudyPlanDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateStudyPlanResponse> ExecuteAsync(
        Guid tenantId, Guid id, UpdateStudyPlanRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);
        if (entity is null)
        {
            throw new StudyPlanNotFoundException(id);
        }

        entity.UpdateDetails(request.Code, request.Name, request.Version, request.EffectiveFrom);

        var integrationEvent = new StudyPlanUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = entity.TenantId,
            StudyPlanId = entity.Id,
            EducationalProgramId = entity.EducationalProgramId,
            Code = entity.Code,
            Name = entity.Name,
            Version = entity.Version,
            EffectiveFrom = entity.EffectiveFrom,
            Status = entity.Status,
            ContractVersion = 1
        };

        await _dataStore.UpdateStudyPlanWithOutboxAsync(entity, integrationEvent, cancellationToken);

        return new UpdateStudyPlanResponse
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            EducationalProgramId = entity.EducationalProgramId,
            Code = entity.Code,
            Name = entity.Name,
            Version = entity.Version,
            EffectiveFrom = entity.EffectiveFrom,
            Status = entity.Status,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow
        };
    }
}