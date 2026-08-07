using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
using SIA.AcademicService.Contracts.Responses.EducationalProgramsResponse;

namespace SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;

public sealed class UpdateEducationalProgramsUseCase
{
    private readonly IEducationalProgramDataStore _dataStore;

    public UpdateEducationalProgramsUseCase(IEducationalProgramDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateEducationalProgramsResponse> ExecuteAsync(
        Guid tenantId, Guid id, UpdateEducationalProgramsRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if (entity is null)
        {
            throw new EducationalProgramNotFoundException(id);
        }

        entity.UpdateDetails(request.Code, request.Name, request.Level);

        var integrationEvent = new EducationalProgramUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = entity.TenantId,
            EducationalProgramId = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Level = entity.Level,
            Status = entity.Status,
            Version = 1
        };

        await _dataStore.UpdateEducationalProgramWithOutboxAsync(entity, integrationEvent, cancellationToken);

        return new UpdateEducationalProgramsResponse
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Code = entity.Code,
            Name = entity.Name,
            Level = entity.Level,
            Status = entity.Status,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow
        };
    }
}