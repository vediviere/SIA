using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class EducationalProgramsDataStore : IEducationalProgramsDataStore
{
    private readonly AcademicDbContext _dbContext;

    public EducationalProgramsDataStore(
        AcademicDbContext dbContext)
    {
        _dbContext = dbContext; 
    }

    public Task<bool> EducationalProgramCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return _dbContext.EducationalPrograms.AnyAsync(educationalPrograms => educationalPrograms.TenantId == tenantId && educationalPrograms.Code == code, cancellationToken);
    }

    public async Task AddEducationalProgramWithOutboxAsync(EducationalPrograms educationalPrograms, EducationalProgramCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);

        var eventType = $"{nameof(EducationalProgramCreatedIntegrationEvent)}.v{integrationEvent.Version}";

        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.EducationalPrograms.AddAsync(educationalPrograms, cancellationToken);

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

    public async Task<EducationalPrograms?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.EducationalPrograms.FirstOrDefaultAsync(educationalProgram => educationalProgram.Id == id, cancellationToken);
    }

    public async Task Update(EducationalPrograms educationalPrograms, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
