namespace SIA.SchedulingService.Domain.Entities.Academics;

public sealed class AcademicLoadGroup
{
    private AcademicLoadGroup()
    {
    }

    public AcademicLoadGroup(
        Guid tenantId,
        Guid academicLoadId,
        Guid groupId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenantId es obligatorio.", nameof(tenantId));
        }

        if (academicLoadId == Guid.Empty)
        {
            throw new ArgumentException("La carga académica es obligatoria.", nameof(academicLoadId));
        }

        if (groupId == Guid.Empty)
        {
            throw new ArgumentException("El grupo es obligatorio.", nameof(groupId));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        AcademicLoadId = academicLoadId;
        GroupId = groupId;
        Hours = 2; // La asignación de horas es predeterminada a 2, ya que cada grupo tiene un máximo de 2 horas de tutoría por semana.
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid AcademicLoadId { get; private set; }
    public Guid GroupId { get; private set; }
    public int Hours { get; private set; }
}