using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;
using System.Text.Json;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class StudyPlanSubjectDataStore : IStudyPlanSubjectDataStore
{
    private readonly AcademicDbContext _dbContext;

    public StudyPlanSubjectDataStore(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> StudyPlanSubjectExistsAsync(Guid tenantId, Guid studyPlanId, Guid subjectId, CancellationToken cancellationToken)
    {
        return _dbContext.StudyPlanSubjects.AnyAsync(
            sps => sps.TenantId == tenantId && sps.StudyPlanId == studyPlanId && sps.SubjectId == subjectId,
            cancellationToken);
    }

    public Task<StudyPlanSubject?> GetStudyPlanSubjectByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.StudyPlanSubjects.FirstOrDefaultAsync(
            sps => sps.TenantId == tenantId && sps.Id == id,
            cancellationToken);
    }

    public async Task AddStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.StudyPlanSubjectCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.StudyPlanSubjects.AddAsync(studyPlanSubject, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.StudyPlanSubjectUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.StudyPlanSubjects.Update(studyPlanSubject);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.StudyPlanSubjectDeletedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.StudyPlanSubjects.Update(studyPlanSubject);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.StudyPlanSubjectRestoredV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.StudyPlanSubjects.Update(studyPlanSubject);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}