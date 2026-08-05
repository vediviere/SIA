using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Contracts.Requests.Subjects;
using SIA.AcademicService.Contracts.Responses.Subjects;
using SIA.AcademicService.Application.Common.Exceptions;


namespace SIA.AcademicService.Application.UseCases.Subjects;

public sealed class UpdateSubjectUseCase
{
    private readonly ISubjectDataStore _subjectDataStore;

    public UpdateSubjectUseCase(ISubjectDataStore subjectDataStore)
    {
        _subjectDataStore = subjectDataStore;
    }

    public async Task<UpdateSubjectResponse> ExecuteAsync(
        Guid tenantId,
        Guid subjectId,
        UpdateSubjectRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var subject = await _subjectDataStore.GetSubjectByIdAsync(tenantId, subjectId, cancellationToken);

        if (subject is null)
        {
            //throw new InvalidOperationException($"No se encontró la asignatura con Id {subjectId}.");
            throw new SubjectNotFoundException(subjectId);
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (subject.Code != normalizedCode)
        {
            var codeExists = await _subjectDataStore.SubjectCodeExistsAsync(tenantId, normalizedCode, cancellationToken);

              if (codeExists)
              {
                throw new DuplicateSubjectCodeException(normalizedCode);
              }
    }

        subject.Update(
            normalizedCode,
            request.Name,
            request.Semester,
            request.TheoryHours,
            request.PracticeHours,
            request.Credits);

        var integrationEvent = new SubjectUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = subject.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = subject.TenantId,
            SubjectId = subject.Id,
            Code = subject.Code,
            Name = subject.Name,
            Semester = subject.Semester,
            TheoryHours = subject.TheoryHours,
            PracticeHours = subject.PracticeHours,
            Credits = subject.Credits,
            Status = subject.Status,
            Version = 1
        };

        await _subjectDataStore.UpdateSubjectWithOutboxAsync(subject, integrationEvent, cancellationToken);

        return new UpdateSubjectResponse
        {
            Id = subject.Id,
            TenantId = subject.TenantId,
            Code = subject.Code,
            Name = subject.Name,
            Semester = subject.Semester,
            TheoryHours = subject.TheoryHours,
            PracticeHours = subject.PracticeHours,
            Credits = subject.Credits,
            Status = subject.Status,
            UpdatedAtUtc = subject.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
