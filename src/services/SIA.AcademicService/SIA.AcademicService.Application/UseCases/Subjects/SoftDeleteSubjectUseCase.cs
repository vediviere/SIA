using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;

namespace SIA.AcademicService.Application.UseCases.Subjects;

public sealed class SoftDeleteSubjectUseCase
{
    private readonly ISubjectDataStore _subjectDataStore;

    public SoftDeleteSubjectUseCase(ISubjectDataStore subjectDataStore)
    {
        _subjectDataStore = subjectDataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid subjectId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var subject = await _subjectDataStore.GetSubjectByIdAsync(tenantId, subjectId, cancellationToken);

        if (subject == null)
        {
            throw new InvalidOperationException($"No se encontró la asignatura con Id {subjectId}.");
        }

        subject.SoftDelete();

        var integrationEvent = new SubjectDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = subject.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = subject.TenantId,
            SubjectId = subject.Id,
            Version = 1
        };

        await _subjectDataStore.SoftDeleteSubjectWithOutboxAsync(subject, integrationEvent, cancellationToken);
    }
}