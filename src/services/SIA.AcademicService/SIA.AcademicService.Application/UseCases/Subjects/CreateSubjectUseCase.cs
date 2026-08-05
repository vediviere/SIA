using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Contracts.Requests.Subjects;
using SIA.AcademicService.Contracts.Responses.Subjects;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Application.Common.Exceptions;

namespace SIA.AcademicService.Application.UseCases.Subjects;

public sealed class CreateSubjectUseCase
{
    private readonly ISubjectDataStore _dataStore;

    public CreateSubjectUseCase(ISubjectDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateSubjectResponse> ExecuteAsync(
        CreateSubjectRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _dataStore.SubjectCodeExistsAsync(
            request.TenantId,
            normalizedCode,
            cancellationToken);

        if (codeExists)
        {
          throw new DuplicateSubjectCodeException(normalizedCode);
        }

    var subject = new Subject(
            request.TenantId,
            normalizedCode,
            request.Name,
            request.Semester,
            request.TheoryHours,
            request.PracticeHours,
            request.Credits);

        var integrationEvent = new SubjectCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = subject.CreatedAtUtc,
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

        await _dataStore.AddSubjectWithOutboxAsync(subject, integrationEvent, cancellationToken);

        return new CreateSubjectResponse
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
            CreatedAtUtc = subject.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
