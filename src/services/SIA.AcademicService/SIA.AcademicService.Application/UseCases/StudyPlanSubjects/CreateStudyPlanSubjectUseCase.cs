/*
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.UseCases.StudyPlanSubjects;

public sealed class CreateStudyPlanSubjectUseCase
{
    private readonly IStudyPlanSubjectDataStore _dataStore;

    public CreateStudyPlanSubjectUseCase(IStudyPlanSubjectDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateStudyPlanSubjectResponse> ExecuteAsync(
        CreateStudyPlanSubjectRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var relationExists = await _dataStore.StudyPlanSubjectExistsAsync(
            request.TenantId,
            request.StudyPlanId,
            request.SubjectId,
            cancellationToken);

        if (relationExists)
        {
            throw new InvalidOperationException("La materia ya se encuentra asignada a este plan de estudios.");
        }

        var studyPlanSubject = new StudyPlanSubject(
            request.TenantId,
            request.StudyPlanId,
            request.SubjectId,
            request.Semester,
            request.Credits,
            request.IsRequired);

        var integrationEvent = new StudyPlanSubjectCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = studyPlanSubject.CreatedAtUtc,
            TenantId = studyPlanSubject.TenantId,
            StudyPlanSubjectId = studyPlanSubject.Id,
            StudyPlanId = studyPlanSubject.StudyPlanId,
            SubjectId = studyPlanSubject.SubjectId,
            Semester = studyPlanSubject.Semester,
            Credits = studyPlanSubject.Credits,
            IsRequired = studyPlanSubject.IsRequired,
            Status = studyPlanSubject.Status,
            Version = 1
        };

        await _dataStore.AddStudyPlanSubjectWithOutboxAsync(studyPlanSubject, integrationEvent, cancellationToken);

        return new CreateStudyPlanSubjectResponse
        {
            Id = studyPlanSubject.Id,
            TenantId = studyPlanSubject.TenantId,
            StudyPlanId = studyPlanSubject.StudyPlanId,
            SubjectId = studyPlanSubject.SubjectId,
            Semester = studyPlanSubject.Semester,
            Credits = studyPlanSubject.Credits,
            IsRequired = studyPlanSubject.IsRequired,
            Status = studyPlanSubject.Status,
            CreatedAtUtc = studyPlanSubject.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}

*/