using SIA.AcademicService.Application.Interfaces.DataStores;

public sealed class DeactivateEducationalProgramsUseCase
{
    private readonly IEducationalProgramsDataStore _dataStore;

    public DeactivateEducationalProgramsUseCase(IEducationalProgramsDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"No se encontró un programa educativo con el id {id}.");
        }

        entity.Deactivate();

        await _dataStore.UpdateAsync(entity, cancellationToken);
    }
}