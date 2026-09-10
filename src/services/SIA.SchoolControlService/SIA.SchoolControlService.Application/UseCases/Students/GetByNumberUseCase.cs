using SIA.SchoolControlService.Application.Interfaces.Queries;
using SIA.SchoolControlService.Contracts.Responses.Students;
using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Application.UseCases.Students;

public sealed class GetByNumberUseCase
{
  private readonly IStudentQueries _queries;

  public GetByNumberUseCase(IStudentQueries queries)
  {
    _queries = queries;
  }

  public async Task<StudentResponse?> ExecuteAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
    }

    if (string.IsNullOrWhiteSpace(studentNumber))
    {
      throw new ArgumentException("La matrícula es obligatoria.", nameof(studentNumber));
    }

    var normalizedStudentNumber = studentNumber.Trim().ToUpperInvariant();

    if (normalizedStudentNumber.Length > Student.StudentNumberMaxLength)
    {
      throw new ArgumentException($"La matrícula no puede exceder {Student.StudentNumberMaxLength} caracteres.", nameof(studentNumber));
    }

    var student = await _queries.GetByStudentNumberAsync(tenantId, normalizedStudentNumber, cancellationToken);

    if (student is null)
    {
      return null;
    }

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
