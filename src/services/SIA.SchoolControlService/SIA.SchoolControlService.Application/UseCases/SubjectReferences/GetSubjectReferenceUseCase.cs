using SIA.SchoolControlService.Application.Interfaces;
using SIA.SchoolControlService.Contracts.Responses;

namespace SIA.SchoolControlService.Application.UseCases.SubjectReferences;

public sealed class GetSubjectReferenceUseCase
{
  private readonly ISchoolControlDataStore _dataStore;

  public GetSubjectReferenceUseCase(ISchoolControlDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<SubjectReferenceResponse?> ExecuteAsync(Guid subjectId, CancellationToken cancellationToken)
  {
    if (subjectId == Guid.Empty)
    {
      throw new ArgumentException("El identificador de la materia es obligatorio.", nameof(subjectId));
    }

    var subjectReference = await _dataStore.GetSubjectReferenceAsync(subjectId, cancellationToken);

    if (subjectReference is null)
    {
      return null;
    }

    return new SubjectReferenceResponse
    {
      SubjectId = subjectReference.SubjectId,
      TenantId = subjectReference.TenantId,
      Code = subjectReference.Code,
      Name = subjectReference.Name,
      Credits = subjectReference.Credits,
      Status = subjectReference.Status,
      UpdatedAtUtc = subjectReference.UpdatedAtUtc
    };
  }
}
