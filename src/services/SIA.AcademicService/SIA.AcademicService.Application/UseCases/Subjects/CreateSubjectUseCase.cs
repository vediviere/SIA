using SIA.AcademicService.Application.Interfaces;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.Requests;
using SIA.AcademicService.Contracts.Responses;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.UseCases.Subjects;

public sealed class CreateSubjectUseCase
{
  private readonly IAcademicDataStore _dataStore;

  public CreateSubjectUseCase(IAcademicDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<CreateSubjectResponse> ExecuteAsync(CreateSubjectRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var normalizedCode = request.Code
        .Trim()
        .ToUpperInvariant();

    var codeExists =
        await _dataStore.SubjectCodeExistsAsync(request.TenantId, normalizedCode, cancellationToken);

    if (codeExists)
    {
      throw new InvalidOperationException($"Ya existe una materia con el código {normalizedCode}.");
    }

    var subject = new Subject(request.TenantId, normalizedCode, request.Name, request.Credits);

    var integrationEvent =
        new SubjectCreatedIntegrationEvent
        {
          EventId = Guid.NewGuid(),
          CorrelationId = correlationId,
          OccurredAtUtc = subject.CreatedAtUtc,
          TenantId = subject.TenantId,
          SubjectId = subject.Id,
          Code = subject.Code,
          Name = subject.Name,
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
      Credits = subject.Credits,
      Status = subject.Status,
      CreatedAtUtc = subject.CreatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
