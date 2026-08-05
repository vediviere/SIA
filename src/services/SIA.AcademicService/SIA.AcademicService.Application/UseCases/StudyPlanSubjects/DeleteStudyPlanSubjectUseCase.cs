/*
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;

namespace SIA.AcademicService.Application.UseCases.StudyPlanSubjects;

public sealed class DeleteStudyPlanSubjectUseCase
{
    private readonly IStudyPlanSubjectDataStore _dataStore;

    public DeleteStudyPlanSubjectUseCase(IStudyPlanSubjectDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<DeleteStudyPlanSubjectResponse> ExecuteAsync(
        DeleteStudyPlanSubjectRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var studyPlanSubject = await _dataStore.GetStudyPlanSubjectByIdAsync(
            request.TenantId,
            request.Id,
            cancellationToken);

        if (studyPlanSubject is null)
        {
            throw new InvalidOperationException($"No se encontró la asignación de materia con el ID {request.Id}.");
        }

        studyPlanSubject.SoftDelete();

        var integrationEvent = new StudyPlanSubjectDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = studyPlanSubject.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = studyPlanSubject.TenantId,
            StudyPlanSubjectId = studyPlanSubject.Id,
            Status = studyPlanSubject.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteStudyPlanSubjectWithOutboxAsync(studyPlanSubject, integrationEvent, cancellationToken);

        return new DeleteStudyPlanSubjectResponse
        {
            Id = studyPlanSubject.Id,
            Status = studyPlanSubject.Status,
            UpdatedAtUtc = studyPlanSubject.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
*/