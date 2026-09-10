namespace SIA.SchedulingService.Domain.Entities;

public sealed class AcademicOffering
{
  private AcademicOffering()
  {
  }

  public AcademicOffering(
      Guid tenantId,
      Guid groupId,
      Guid subjectId,
      Guid academicLoadId,
      string offeringStatus)
  {
    if (tenantId == Guid.Empty)
    {
      throw new ArgumentException("El tenantId es obligatorio.", nameof(tenantId));
    }
    if (groupId == Guid.Empty)
    {
      throw new ArgumentException("El grupo es obligatorio.", nameof(groupId));
    }
    if (subjectId == Guid.Empty)
    {
      throw new ArgumentException("La materia es obligatoria.", nameof(subjectId));
    }
    if (academicLoadId == Guid.Empty)
    {
      throw new ArgumentException("La carga academica es obligatoria.", nameof(academicLoadId));
    }
    if (string.IsNullOrWhiteSpace(offeringStatus))
    {
      throw new ArgumentException("El estado de la oferta es obligatorio.", nameof(offeringStatus));
    }

    Id = Guid.NewGuid();
    TenantId = tenantId;
    GroupId = groupId;
    SubjectId = subjectId;
    AcademicLoadId = academicLoadId;
    OfferingStatus = offeringStatus.Trim();
    ClassHours = 0;
    Status = true;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
  public Guid GroupId { get; private set; }
  public Guid SubjectId { get; private set; }
  public Guid AcademicLoadId { get; private set; }
  public string OfferingStatus { get; private set; } = string.Empty;
  public int ClassHours { get; private set; }
  public bool Status { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime? UpdatedAtUtc { get; private set; }

  public void Update(string offeringStatus)
  {
    if (string.IsNullOrWhiteSpace(offeringStatus))
    {
      throw new ArgumentException("El estado de la oferta es obligatorio.", nameof(offeringStatus));
    }

    OfferingStatus = offeringStatus.Trim();
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void AssignClassHours(int classHours)
  {
    if (classHours <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(classHours), "Las horas frente a grupo deben ser mayores que cero.");
    }

    ClassHours = classHours;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    Status = false;
    UpdatedAtUtc = DateTime.UtcNow;
  }

  public void Activate()
  {
    Status = true;
    UpdatedAtUtc = DateTime.UtcNow;
  }
}
