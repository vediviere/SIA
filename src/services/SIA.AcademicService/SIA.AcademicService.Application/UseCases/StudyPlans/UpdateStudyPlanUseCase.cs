using SIA.AcademicService.Application.Interfaces.DataStores;
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

    public async Task<UpdateStudyPlanResponse> ExecuteAsync(Guid id, UpdateStudyPlanRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"No se encontró un plan de estudios con el id {id}.");
        }

        entity.UpdateDetails(request.Code, request.Name, request.Version, request.EffectiveFrom);

        await _dataStore.UpdateAsync(entity, cancellationToken);

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
