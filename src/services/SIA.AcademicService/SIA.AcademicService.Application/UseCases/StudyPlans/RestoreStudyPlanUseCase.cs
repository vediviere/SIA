using SIA.AcademicService.Application.Interfaces.DataStores;

namespace SIA.AcademicService.Application.UseCases.StudyPlans;
public sealed class RestoreStudyPlanUseCase
{
    private readonly IStudyPlanDataStore _dataStore;

    public RestoreStudyPlanUseCase(IStudyPlanDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"No se encontró un plan de estudios con el id {id}.");
        }

        entity.Activate();
        await _dataStore.UpdateAsync(entity, cancellationToken);
    }
}