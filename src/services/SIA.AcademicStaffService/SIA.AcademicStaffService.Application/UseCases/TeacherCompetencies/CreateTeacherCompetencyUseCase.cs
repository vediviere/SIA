using System;
using System.Threading;
using System.Threading.Tasks;
using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;
using SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency;
using SIA.AcademicStaffService.Contracts.Responses.TeacherCompetency;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies
{
    public sealed class CreateTeacherCompetencyUseCase
    {
        private readonly ITeacherCompetencyDataStore _dataStore;

        public CreateTeacherCompetencyUseCase(ITeacherCompetencyDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<CreateTeacherCompetencyResponse> ExecuteAsync(
            Guid tenantId,
            Guid teacherId,
            CreateTeacherCompetencyRequest request,
            Guid correlationId,
            CancellationToken cancellationToken)
        {
            var alreadyExists = await _dataStore.CompetencyAlreadyExistsForTeacherAsync(
                tenantId,
                teacherId,
                request.CompetencyId,
                cancellationToken);

            if (alreadyExists)
            {
                throw new DuplicateTeacherCompetencyException(teacherId, request.CompetencyId);
            }

            var teacherCompetency = new TeacherCompetency(
                tenantId,
                teacherId,
                request.CompetencyId,
                request.Level,
                request.Stage);

            var integrationEvent = new TeacherCompetencyCreatedIntegrationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = correlationId,
                OccurredAtUtc = teacherCompetency.CreatedAtUtc,
                TenantId = teacherCompetency.TenantId,
                TeacherCompetencyId = teacherCompetency.TeacherCompetencyId,
                TeacherId = teacherCompetency.TeacherId,
                CompetencyId = teacherCompetency.CompetencyId,
                Level = teacherCompetency.Level,
                Stage = teacherCompetency.Stage,
                Status = teacherCompetency.Status,
                Version = 1
            };

            await _dataStore.AddTeacherCompetencyWithOutboxAsync(teacherCompetency, integrationEvent, cancellationToken);

            return new CreateTeacherCompetencyResponse
            {
                TeacherCompetencyId = teacherCompetency.TeacherCompetencyId,
                TenantId = teacherCompetency.TenantId,
                TeacherId = teacherCompetency.TeacherId,
                CompetencyId = teacherCompetency.CompetencyId,
                Level = teacherCompetency.Level,
                Stage = teacherCompetency.Stage,
                Status = teacherCompetency.Status,
                CreatedAtUtc = teacherCompetency.CreatedAtUtc,
                CorrelationId = correlationId
            };
        }
    }
}