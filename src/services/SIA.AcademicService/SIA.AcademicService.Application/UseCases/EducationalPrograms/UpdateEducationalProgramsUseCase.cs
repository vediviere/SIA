using SIA.AcademicService.Application.Interfaces.DataStores;
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

    public async Task<UpdateEducationalProgramsResponse> ExecuteAsync(Guid id, UpdateEducationalProgramsRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dataStore.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"No se encontró un programa educativo con el id {id}.");
        }

        entity.UpdateDetails(request.Code, request.Name, request.Level);

        await _dataStore.UpdateAsync(entity, cancellationToken);

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