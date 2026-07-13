using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class AcademicDataStore : IAcademicDataStore
{
  private readonly AcademicDbContext _dbContext;

  public AcademicDataStore(
      AcademicDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> SubjectCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    return _dbContext.Subjects.AnyAsync(subject => subject.TenantId == tenantId && subject.Code == code,
        cancellationToken);
  }

  public async Task AddSubjectWithOutboxAsync(Subject subject, SubjectCreatedIntegrationEvent integrationEvent,
      CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);

    var eventType =
        $"{nameof(SubjectCreatedIntegrationEvent)}.v{integrationEvent.Version}";

    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await using var transaction =
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    try
    {
      await _dbContext.Subjects.AddAsync(subject, cancellationToken);

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
