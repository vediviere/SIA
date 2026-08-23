using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;

namespace SIA.AcademicService.Application.UseCases.StudyPlanSubjects;

public sealed class UpdateStudyPlanSubjectUseCase
{
    private readonly IStudyPlanSubjectDataStore _dataStore;

    public UpdateStudyPlanSubjectUseCase(IStudyPlanSubjectDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateStudyPlanSubjectResponse> ExecuteAsync(
        Guid tenantId,
        Guid studyPlanSubjectId, 
        UpdateStudyPlanSubjectRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var studyPlanSubject = await _dataStore.GetStudyPlanSubjectByIdAsync(
            tenantId,
            studyPlanSubjectId, 
            cancellationToken);

        if (studyPlanSubject is null)
        {
            throw new StudyPlanSubjectNotFoundException(studyPlanSubjectId); 
        }

        studyPlanSubject.Update(request.Semester, request.Credits, request.IsRequired);

        var integrationEvent = new StudyPlanSubjectUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = studyPlanSubject.UpdatedAtUtc ?? DateTime.UtcNow,
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

        await _dataStore.UpdateStudyPlanSubjectWithOutboxAsync(studyPlanSubject, integrationEvent, cancellationToken);

        return new UpdateStudyPlanSubjectResponse
        {
            Id = studyPlanSubject.Id,
            TenantId = studyPlanSubject.TenantId,
            StudyPlanId = studyPlanSubject.StudyPlanId,
            SubjectId = studyPlanSubject.SubjectId,
            Semester = studyPlanSubject.Semester,
            Credits = studyPlanSubject.Credits,
            IsRequired = studyPlanSubject.IsRequired,
            Status = studyPlanSubject.Status,
            UpdatedAtUtc = studyPlanSubject.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}