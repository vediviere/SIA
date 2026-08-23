using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.UseCases.StudyPlanSubjects;

public sealed class SoftDeleteStudyPlanSubjectUseCase
{
    private readonly IStudyPlanSubjectDataStore _dataStore;

    public SoftDeleteStudyPlanSubjectUseCase(IStudyPlanSubjectDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid studyPlanSubjectId,
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

        studyPlanSubject.SoftDelete();

        var integrationEvent = new StudyPlanSubjectDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = studyPlanSubject.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = studyPlanSubject.TenantId,
            StudyPlanSubjectId = studyPlanSubject.Id,
            StudyPlanId = studyPlanSubject.StudyPlanId, 
            SubjectId = studyPlanSubject.SubjectId,    
            Status = studyPlanSubject.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteStudyPlanSubjectWithOutboxAsync(studyPlanSubject, integrationEvent, cancellationToken);
    }
}