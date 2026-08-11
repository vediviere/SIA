using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;

namespace SIA.AcademicStaffService.Application.UseCases.Professors;

public sealed class DeactivateProfessorUseCase
{
    private readonly IProfessorDataStore _dataStore;

    public DeactivateProfessorUseCase(IProfessorDataStore dataStore)
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
            throw new ProfessorNotFoundException(professorId);
        }

        professor.Deactivate();

        var integrationEvent = new ProfessorDeactivatedIntegrationEvent
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