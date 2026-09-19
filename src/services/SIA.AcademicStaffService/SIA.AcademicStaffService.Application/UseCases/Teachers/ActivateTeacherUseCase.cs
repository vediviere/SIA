using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Teacher;

namespace SIA.AcademicStaffService.Application.UseCases.Teachers;

public sealed class ActivateTeacherUseCase
{
    private readonly ITeacherDataStore _dataStore;

    public ActivateTeacherUseCase(ITeacherDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid teacherId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var teacher = await _dataStore.GetTeacherByIdAsync(tenantId, teacherId, cancellationToken);

        if (teacher is null)
        {
            throw new TeacherNotFoundException(teacherId);
        }

        teacher.Activate();

        var integrationEvent = new TeacherActivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = teacher.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = teacher.TenantId,
            TeacherId = teacher.Id,
            Version = 1
        };

        await _dataStore.ActivateTeacherWithOutboxAsync(teacher, integrationEvent, cancellationToken);
    }
}