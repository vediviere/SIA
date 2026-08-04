using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.Entities;
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
        var eventType = $"{nameof(StudyPlanSubjectCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.StudyPlanSubjects.AddAsync(studyPlanSubject, cancellationToken);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(StudyPlanSubjectUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.StudyPlanSubjects.Update(studyPlanSubject);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task SoftDeleteStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(StudyPlanSubjectDeletedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.StudyPlanSubjects.Update(studyPlanSubject);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RestoreStudyPlanSubjectWithOutboxAsync(StudyPlanSubject studyPlanSubject, StudyPlanSubjectRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(StudyPlanSubjectRestoredIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.StudyPlanSubjects.Update(studyPlanSubject);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}