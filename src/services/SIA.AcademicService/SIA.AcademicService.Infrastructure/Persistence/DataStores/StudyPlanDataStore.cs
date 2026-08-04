using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class StudyPlanDataStore : IStudyPlanDataStore
{
    private readonly AcademicDbContext _dbContext;

    public StudyPlanDataStore(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> StudyPlanCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return _dbContext.StudyPlans.AnyAsync(studyPlan => studyPlan.TenantId == tenantId && studyPlan.Code == code, cancellationToken);
    }

    public async Task AddStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(StudyPlanCreatedIntegrationEvent)}.v{integrationEvent.ContractVersion}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.StudyPlans.AddAsync(studyPlan, cancellationToken);
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

    public async Task<StudyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.StudyPlans.FirstOrDefaultAsync(studyPlan => studyPlan.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(StudyPlan studyPlan, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}