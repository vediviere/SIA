using SIA.SchoolControlService.Application.Common.Exceptions.Students;
using SIA.SchoolControlService.Application.Interfaces.DataStores;
using SIA.SchoolControlService.Contracts.Requests.Students;
using SIA.SchoolControlService.Contracts.Responses.Students;
using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Application.UseCases.Students;

public sealed class LinkUseCase
{
  private readonly IStudentDataStore _dataStore;

  public LinkUseCase(IStudentDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<LinkResponse> ExecuteAsync(LinkRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    if (request.TenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenant es obligatorio.", nameof(request.TenantId));
    }

    if (request.UserId == Guid.Empty)
    {
      throw new ArgumentException("El usuario es obligatorio.", nameof(request.UserId));
    }

    if (string.IsNullOrWhiteSpace(request.StudentNumber))
    {
      throw new ArgumentException("La matrícula es obligatoria.", nameof(request.StudentNumber));
    }

    var studentNumber = request.StudentNumber.Trim().ToUpperInvariant();

    if (studentNumber.Length > Student.StudentNumberMaxLength)
    {
      throw new ArgumentException($"La matrícula no puede exceder {Student.StudentNumberMaxLength} caracteres.", nameof(request.StudentNumber));
    }

    var student = await _dataStore.GetByStudentNumberAsync(request.TenantId, studentNumber, cancellationToken);

    if (student is null)
    {
      throw new StudentNotFoundException(studentNumber);
    }

    if (!student.Status)
    {
      throw new InactiveException(studentNumber);
    }

    if (student.UserId.HasValue && student.UserId != request.UserId)
    {
      throw new LinkedException(studentNumber);
    }

    var linked = await _dataStore.TryLinkUserAsync(student, request.UserId, cancellationToken);

    if (!linked)
    {
      throw new LinkedException(studentNumber);
    }

    return new LinkResponse
    {
      StudentId = student.Id,
      UserId = request.UserId,
      StudentNumber = student.StudentNumber,
      Linked = true
    };
  }
}
