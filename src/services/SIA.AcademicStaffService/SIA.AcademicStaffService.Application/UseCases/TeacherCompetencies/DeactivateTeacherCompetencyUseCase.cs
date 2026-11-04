using System;
using System.Threading;
using System.Threading.Tasks;
using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;

namespace SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies
{
    public sealed class DeactivateTeacherCompetencyUseCase
    {
        private readonly ITeacherCompetencyDataStore _dataStore;

        public DeactivateTeacherCompetencyUseCase(ITeacherCompetencyDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task ExecuteAsync(
            Guid tenantId,
            Guid teacherCompetencyId,
            Guid correlationId,
            CancellationToken cancellationToken)
        {
            var teacherCompetency = await _dataStore.GetTeacherCompetencyByIdAsync(tenantId, teacherCompetencyId, cancellationToken);

            if (teacherCompetency is null)
            {
                throw new TeacherCompetencyNotFoundException(teacherCompetencyId);
            }

            teacherCompetency.Deactivate();

            var integrationEvent = new TeacherCompetencyDeactivatedIntegrationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = correlationId,
                OccurredAtUtc = teacherCompetency.UpdatedAtUtc ?? DateTime.UtcNow,
                TenantId = teacherCompetency.TenantId,
                TeacherCompetencyId = teacherCompetency.TeacherCompetencyId,
                Version = 1
            };

            await _dataStore.DeactivateTeacherCompetencyWithOutboxAsync(teacherCompetency, integrationEvent, cancellationToken);
        }
    }
}