namespace SIA.SchedulingService.Domain.Entities;

public sealed class TeachingSupportHour
{
    private TeachingSupportHour()
    {
    }

    public TeachingSupportHour(
        Guid tenantId,
        Guid activityId,
        Guid academicLoadId,
        int hours)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }
        if (activityId == Guid.Empty)
        {
            throw new ArgumentException("La actividad de apoyo es obligatoria.", nameof(activityId));
        }
        if (academicLoadId == Guid.Empty)
        {
            throw new ArgumentException("La carga académica es obligatoria.", nameof(academicLoadId));
        }
        if (hours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Las horas deben ser mayores que cero.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ActivityId = activityId;
        AcademicLoadId = academicLoadId;
        Hours = hours;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ActivityId { get; private set; }
    public Guid AcademicLoadId { get; private set; }
    public int Hours { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(int hours)
    {
        if (hours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Las horas deben ser mayores que cero.");
        }

        Hours = hours;
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