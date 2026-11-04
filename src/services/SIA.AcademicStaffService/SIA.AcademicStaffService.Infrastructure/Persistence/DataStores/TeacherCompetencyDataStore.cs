using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.DataStores
{
    public sealed class TeacherCompetencyDataStore : ITeacherCompetencyDataStore
    {
        private readonly AcademicStaffDbContext _dbContext;

        public TeacherCompetencyDataStore(AcademicStaffDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> CompetencyAlreadyExistsForTeacherAsync(Guid tenantId, Guid teacherId, Guid competencyId, CancellationToken cancellationToken)
        {
            return _dbContext.Set<TeacherCompetency>().AnyAsync(
                x => x.TenantId == tenantId && x.TeacherId == teacherId && x.CompetencyId == competencyId,
                cancellationToken);
        }

        public Task<TeacherCompetency?> GetTeacherCompetencyByIdAsync(Guid tenantId, Guid teacherCompetencyId, CancellationToken cancellationToken)
        {
            return _dbContext.Set<TeacherCompetency>().FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.TeacherCompetencyId == teacherCompetencyId,
                cancellationToken);
        }

        public async Task AddTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Serialize(integrationEvent);
            var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherCompetencyCreatedV1, payload, integrationEvent.CorrelationId);

            await _dbContext.Set<TeacherCompetency>().AddAsync(teacherCompetency, cancellationToken);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Serialize(integrationEvent);
            var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherCompetencyUpdatedV1, payload, integrationEvent.CorrelationId);

            _dbContext.Set<TeacherCompetency>().Update(teacherCompetency);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task ActivateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Serialize(integrationEvent);
            var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherCompetencyActivatedV1, payload, integrationEvent.CorrelationId);

            _dbContext.Set<TeacherCompetency>().Update(teacherCompetency);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Serialize(integrationEvent);
            var outboxMessage = new OutboxMessage(AcademicStaffIntegrationEventTypes.TeacherCompetencyDeactivatedV1, payload, integrationEvent.CorrelationId);

            _dbContext.Set<TeacherCompetency>().Update(teacherCompetency);
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}