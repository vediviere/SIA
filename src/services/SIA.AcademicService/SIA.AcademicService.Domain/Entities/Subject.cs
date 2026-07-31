namespace SIA.AcademicService.Domain.Entities;

public sealed class Subject
{
  private Subject()
  {
  }

  public Subject(
      Guid tenantId,
      Guid studyPlanId,
      string code,
      string name,
      int semester,
      int theoryHours,
      int practiceHours,
      int credits)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException(
          "El tenant es obligatorio.",
          nameof(tenantId));
    }

    if (studyPlanId == Guid.Empty)
        {
            throw new ArgumentException(
                "El plan de estudio es obligatorio.",
                nameof(studyPlanId));
        }

    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException(
          "El código de la materia es obligatorio.",
          nameof(code));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException(
          "El nombre de la materia es obligatorio.",
          nameof(name));
    }

    if (semester <= 0) 
        {
            throw new ArgumentOutOfRangeException
                (nameof(semester), 
                "El semestre debe ser mayor a cero." );
        }

    if (theoryHours < 0) 
        {
            throw new ArgumentOutOfRangeException
                (nameof(theoryHours),
                "Las horas teóricas no puedes ser negativas.");
        }

    if (practiceHours < 0) 
        {
            throw new ArgumentOutOfRangeException
                (nameof(practiceHours),
                "Las horas prácticas no pueden ser negativas."
                );
        }

    if (credits <= 0)
    {
      throw new ArgumentOutOfRangeException(
          nameof(credits),
          "Los créditos deben ser mayores que cero.");
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    StudyPlanId = studyPlanId;
    Code = code.Trim().ToUpperInvariant();
    Name = name.Trim();
    Semester = semester;
    TheoryHours = theoryHours;
    PracticeHours = practiceHours;
    Credits = credits;
    Status = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid StudyPlanId { get; private set; }

  public Guid TenantId { get; private set; }

  public string Code { get; private set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;

  public int Semester { get; private set; }

  public int TheoryHours { get; private set; }

  public int PracticeHours { get; private set; }

  public int Credits { get; private set; }

  public bool Status { get; private set; } 

  public DateTime CreatedAtUtc { get; private set; } 

  public DateTime? UpdatedAtUtc { get; private set; }
}
