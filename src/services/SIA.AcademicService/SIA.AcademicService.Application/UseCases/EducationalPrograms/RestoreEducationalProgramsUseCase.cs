using SIA.AcademicService.Application.Interfaces.DataStores;
using System;
using System.Collections.Generic;
using System.Text;

public sealed class RestoreEducationalProgramsUseCase
{
    private readonly IEducationalProgramDataStore _dataStore;

    public RestoreEducationalProgramsUseCase(IEducationalProgramDataStore dataStore)
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

        entity.Activate();

        await _dataStore.UpdateAsync(entity, cancellationToken);
    }
}
