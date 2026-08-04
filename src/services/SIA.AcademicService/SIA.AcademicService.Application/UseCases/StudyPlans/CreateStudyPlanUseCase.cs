using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.Requests.StudyPlans;
using SIA.AcademicService.Contracts.Responses.StudyPlans;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.UseCases.StudyPlans;

public sealed class CreateStudyPlanUseCase
{
    private readonly IStudyPlanDataStore _dataStore;

    public CreateStudyPlanUseCase(IStudyPlanDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateStudyPlanResponse> ExecuteAsync(CreateStudyPlanRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _dataStore.StudyPlanCodeExistsAsync(request.TenantId, normalizedCode, cancellationToken);
        if (codeExists)
        {
            throw new InvalidOperationException($"Ya existe un plan de estudios con el código {normalizedCode}.");
        }

        var studyPlan = new StudyPlan(request.TenantId, request.EducationalProgramId, normalizedCode, request.Name, request.Version, request.EffectiveFrom);

        var integrationEvent = new StudyPlanCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = studyPlan.CreatedAtUtc,
            TenantId = studyPlan.TenantId,
            StudyPlanId = studyPlan.Id,
            EducationalProgramId = studyPlan.EducationalProgramId,
            Code = studyPlan.Code,
            Name = studyPlan.Name,
            Version = studyPlan.Version,
            EffectiveFrom = studyPlan.EffectiveFrom,
            Status = studyPlan.Status,
            ContractVersion = 1
        };

        await _dataStore.AddStudyPlanWithOutboxAsync(studyPlan, integrationEvent, cancellationToken);

        return new CreateStudyPlanResponse
        {
            Id = studyPlan.Id,
            TenantId = studyPlan.TenantId,
            EducationalProgramId = studyPlan.EducationalProgramId,
            Code = studyPlan.Code,
            Name = studyPlan.Name,
            Version = studyPlan.Version,
            EffectiveFrom = studyPlan.EffectiveFrom,
            Status = studyPlan.Status,
            CreatedAtUtc = studyPlan.CreatedAtUtc
        };
    }
}
