using System;
using System.Threading;
using System.Threading.Tasks;
using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;

namespace SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies
{
    public sealed class ActivateTeacherCompetencyUseCase
    {
        private readonly ITeacherCompetencyDataStore _dataStore;

        public ActivateTeacherCompetencyUseCase(ITeacherCompetencyDataStore dataStore)
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

            teacherCompetency.Activate();

            var integrationEvent = new TeacherCompetencyActivatedIntegrationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = correlationId,
                OccurredAtUtc = teacherCompetency.UpdatedAtUtc ?? DateTime.UtcNow,
                TenantId = teacherCompetency.TenantId,
                TeacherCompetencyId = teacherCompetency.TeacherCompetencyId,
                Version = 1
            };

            await _dataStore.ActivateTeacherCompetencyWithOutboxAsync(teacherCompetency, integrationEvent, cancellationToken);
        }
    }
}