using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;

namespace SIA.AcademicStaffService.Application.UseCases.Professors;

public sealed class DeactivateTeacherUseCase
{
    private readonly ITeacherDataStore _dataStore;

    public DeactivateTeacherUseCase(ITeacherDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid professorId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var professor = await _dataStore.GetProfessorByIdAsync(tenantId, professorId, cancellationToken);

        if (professor is null)
        {
            throw new TeacherNotFoundException(professorId);
        }

        professor.Deactivate();

        var integrationEvent = new TeacherDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = professor.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = professor.TenantId,
            ProfessorId = professor.Id,
            Version = 1
        };

        await _dataStore.DeactivateProfessorWithOutboxAsync(professor, integrationEvent, cancellationToken);
    }
}