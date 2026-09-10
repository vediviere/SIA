using SIA.SchoolControlService.Application.Common.Exceptions.Students;
using SIA.SchoolControlService.Application.Interfaces.DataStores;
using SIA.SchoolControlService.Contracts.Requests.Students;
using SIA.SchoolControlService.Contracts.Responses.Students;
using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Application.UseCases.Students;

public sealed class CreateUseCase
{
  private readonly IStudentDataStore _dataStore;

  public CreateUseCase(IStudentDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<StudentResponse> ExecuteAsync(Guid tenantId, CreateRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var student = new Student(
      tenantId,
      request.StudentNumber,
      request.FirstName,
      request.PaternalLastName,
      request.MaternalLastName);

    var numberExists = await _dataStore.ExistsByStudentNumberAsync(student.TenantId, student.StudentNumber, cancellationToken);

    if (numberExists)
    {
      throw new DuplicateNumberException(student.StudentNumber);
    }

    await _dataStore.AddAsync(student, cancellationToken);

    return new StudentResponse
    {
      StudentId = student.Id,
      TenantId = student.TenantId,
      StudentNumber = student.StudentNumber,
      FirstName = student.FirstName,
      PaternalLastName = student.PaternalLastName,
      MaternalLastName = student.MaternalLastName,
      Status = student.Status,
      CreatedAtUtc = student.CreatedAtUtc,
      UpdatedAtUtc = student.UpdatedAtUtc
    };
  }
}
