namespace SIA.SchoolControlService.Domain.Entities;

public sealed class Student
{
  public const int StudentNumberMaxLength = 50;
  public const int FirstNameMaxLength = 100;
  public const int LastNameMaxLength = 100;

  private Student()
  {
  }

  public Student(Guid tenantId, string studentNumber, string firstName, string paternalLastName, string maternalLastName)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
    }

    if (string.IsNullOrWhiteSpace(studentNumber))
    {
      throw new ArgumentException("La matrícula es obligatoria.", nameof(studentNumber));
    }

    if (string.IsNullOrWhiteSpace(firstName))
    {
      throw new ArgumentException("El nombre es obligatorio.", nameof(firstName));
    }

    if (string.IsNullOrWhiteSpace(paternalLastName))
    {
      throw new ArgumentException("El apellido paterno es obligatorio.", nameof(paternalLastName));
    }

    if (string.IsNullOrWhiteSpace(maternalLastName))
    {
      throw new ArgumentException("El apellido materno es obligatorio.", nameof(maternalLastName));
    }

    var normalizedStudentNumber = studentNumber.Trim().ToUpperInvariant();
    var normalizedFirstName = firstName.Trim();
    var normalizedPaternalLastName = paternalLastName.Trim();
    var normalizedMaternalLastName = maternalLastName.Trim();

    if (normalizedStudentNumber.Length > StudentNumberMaxLength)
    {
      throw new ArgumentException($"La matrícula no puede exceder {StudentNumberMaxLength} caracteres.", nameof(studentNumber));
    }

    if (normalizedFirstName.Length > FirstNameMaxLength)
    {
      throw new ArgumentException($"El nombre no puede exceder {FirstNameMaxLength} caracteres.", nameof(firstName));
    }

    if (normalizedPaternalLastName.Length > LastNameMaxLength)
    {
      throw new ArgumentException($"El apellido paterno no puede exceder {LastNameMaxLength} caracteres.", nameof(paternalLastName));
    }

    if (normalizedMaternalLastName.Length > LastNameMaxLength)
    {
      throw new ArgumentException($"El apellido materno no puede exceder {LastNameMaxLength} caracteres.", nameof(maternalLastName));
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    StudentNumber = normalizedStudentNumber;
    FirstName = normalizedFirstName;
    PaternalLastName = normalizedPaternalLastName;
    MaternalLastName = normalizedMaternalLastName;
    Status = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
  public string StudentNumber { get; private set; } = string.Empty;
  public string FirstName { get; private set; } = string.Empty;
  public string PaternalLastName { get; private set; } = string.Empty;
  public string MaternalLastName { get; private set; } = string.Empty;
  public bool Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }
}
