using System;
using System.Threading;
using System.Threading.Tasks;
using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;
using SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency;
using SIA.AcademicStaffService.Contracts.Responses.TeacherCompetency;

namespace SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies
{
    public sealed class UpdateTeacherCompetencyUseCase
    {
        private readonly ITeacherCompetencyDataStore _dataStore;

        public UpdateTeacherCompetencyUseCase(ITeacherCompetencyDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<UpdateTeacherCompetencyResponse> ExecuteAsync(
            Guid tenantId,
            Guid teacherCompetencyId,
            UpdateTeacherCompetencyRequest request,
            Guid correlationId,
            CancellationToken cancellationToken)
        {
            var teacherCompetency = await _dataStore.GetTeacherCompetencyByIdAsync(tenantId, teacherCompetencyId, cancellationToken);

            if (teacherCompetency is null)
            {
                throw new TeacherCompetencyNotFoundException(teacherCompetencyId);
            }

            teacherCompetency.Update(
                request.Level,
                request.Stage);

            var integrationEvent = new TeacherCompetencyUpdatedIntegrationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = correlationId,
                OccurredAtUtc = teacherCompetency.UpdatedAtUtc ?? DateTime.UtcNow,
                TenantId = teacherCompetency.TenantId,
                TeacherCompetencyId = teacherCompetency.TeacherCompetencyId,
                TeacherId = teacherCompetency.TeacherId,
                CompetencyId = teacherCompetency.CompetencyId,
                Level = teacherCompetency.Level,
                Stage = teacherCompetency.Stage,
                Status = teacherCompetency.Status,
                Version = 1
            };

            await _dataStore.UpdateTeacherCompetencyWithOutboxAsync(teacherCompetency, integrationEvent, cancellationToken);

            return new UpdateTeacherCompetencyResponse
            {
                TeacherCompetencyId = teacherCompetency.TeacherCompetencyId,
                TenantId = teacherCompetency.TenantId,
                TeacherId = teacherCompetency.TeacherId,
                CompetencyId = teacherCompetency.CompetencyId,
                Level = teacherCompetency.Level,
                Stage = teacherCompetency.Stage,
                Status = teacherCompetency.Status,
                UpdatedAtUtc = teacherCompetency.UpdatedAtUtc,
                CorrelationId = correlationId
            };
        }
    }
}