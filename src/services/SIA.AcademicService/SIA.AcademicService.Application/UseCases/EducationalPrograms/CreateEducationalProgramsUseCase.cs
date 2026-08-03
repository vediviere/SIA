using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
using SIA.AcademicService.Contracts.Responses.EducationalProgramsResponse;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;

public sealed class CreateEducationalProgramsUseCase
{
    private readonly IEducationalProgramsDataStore _dataStore;

    public CreateEducationalProgramsUseCase(IEducationalProgramsDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateEducationalProgramsResponse> ExecuteAsync(CreateEducationalProgramsRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _dataStore.EducationalProgramCodeExistsAsync(request.TenantId, normalizedCode, cancellationToken);

        if (codeExists)
        {
            throw new InvalidOperationException($"Ya existe un Programa Educativo con este codigo{normalizedCode}.");
        }

        var educationalPrograms = new EducationalProgram(request.TenantId, normalizedCode, request.Name, request.Level);

        var integrationEvent = new EducationalProgramCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = educationalPrograms.CreatedAtUtc,
            TenantId = educationalPrograms.TenantId,
            EducationalProgramId = educationalPrograms.Id,
            Code =educationalPrograms.Code,
            Name = educationalPrograms.Name,
            Level = educationalPrograms.Level,
            Status = educationalPrograms.Status,
            Version = 1
        };

        await _dataStore.AddEducationalProgramWithOutboxAsync(educationalPrograms, integrationEvent, cancellationToken);

        return new CreateEducationalProgramsResponse
        {
            Id = educationalPrograms.Id,
            TenantId = educationalPrograms.TenantId,
            Code = educationalPrograms.Code,
            Name = educationalPrograms.Name,
            Level = educationalPrograms.Level,
            Status = educationalPrograms.Status,
            CreatedAtUtc = educationalPrograms.CreatedAtUtc
        };
    }
}
