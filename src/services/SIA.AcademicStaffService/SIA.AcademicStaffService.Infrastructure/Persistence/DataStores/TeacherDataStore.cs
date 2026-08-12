using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.AcademicStaffService.Infrastructure.Persistence.Entities;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.DataStores;

public sealed class TeacherDataStore : ITeacherDataStore
{
    private readonly AcademicStaffDbContext _dbContext;

    public TeacherDataStore(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> PersonAlreadyProfessorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
    {
        return _dbContext.Teacher.AnyAsync(
            teacher => teacher.TenantId == tenantId && teacher.PersonId == personId,
            cancellationToken);
    }

    public Task<Teacher?> GetProfessorByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken)
    {
        return _dbContext.Teacher.FirstOrDefaultAsync(
            teacher => teacher.TenantId == tenantId && teacher.Id == professorId,
            cancellationToken);
    }

    public async Task AddProfessorWithOutboxAsync(Teacher teacher, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeacherCreatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.Teacher.AddAsync(teacher, cancellationToken);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateProfessorWithOutboxAsync(Teacher teacher, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeacherUpdatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.Teacher.Update(teacher);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task ActivateProfessorWithOutboxAsync(Teacher teacher, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeacherActivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.Teacher.Update(teacher);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeactivateProfessorWithOutboxAsync(Teacher teacher, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = $"{nameof(TeacherDeactivatedIntegrationEvent)}.v{integrationEvent.Version}";
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.Teacher.Update(teacher);
            await _dbContext.OutboxMessage.AddAsync(outboxMessage, cancellationToken);
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