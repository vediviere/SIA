using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

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
        return _dbContext.Teachers.AnyAsync(
            teacher => teacher.TenantId == tenantId && teacher.PersonId == personId,
            cancellationToken);
    }

    public Task<Teacher?> GetProfessorByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken)
    {
        return _dbContext.Teachers.FirstOrDefaultAsync(
            teacher => teacher.TenantId == tenantId && teacher.Id == professorId,
            cancellationToken);
    }

    public async Task AddProfessorWithOutboxAsync(Teacher teacher, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherCreatedV1, payload, integrationEvent.CorrelationId);

        await _dbContext.Teachers.AddAsync(teacher, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProfessorWithOutboxAsync(Teacher teacher, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherUpdatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Teachers.Update(teacher);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateProfessorWithOutboxAsync(Teacher teacher, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherActivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Teachers.Update(teacher);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateProfessorWithOutboxAsync(Teacher teacher, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherDeactivatedV1, payload, integrationEvent.CorrelationId);

        _dbContext.Teachers.Update(teacher);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}