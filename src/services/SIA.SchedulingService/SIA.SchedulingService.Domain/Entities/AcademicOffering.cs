using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

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
        Guid academicLoadId)
    {
        if(tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenantId es obligatorio.", nameof(tenantId));
        }

        if(groupId == Guid.Empty)
        {
            throw new ArgumentException("El grupo es obligatorio.", nameof (groupId));
        }

        if(subjectId == Guid.Empty)
        {
            throw new ArgumentException("La materia es obligatoria.", nameof(subjectId));
        }

        if (academicLoadId == Guid.Empty)
        {
            throw new ArgumentException("La carga academica es obligatoria.", nameof(academicLoadId));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        GroupId = groupId;
        SubjectId = subjectId;
        AcademicLoadId = academicLoadId;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid GroupId { get; private set; }

    public Guid SubjectId { get; private set; }

    public Guid AcademicLoadId { get; private set; }

    public bool Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(Guid groupId, Guid subjectId, Guid academicLoadId)
    {
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
            throw new ArgumentException("La carga académica es obligatoria.", nameof(academicLoadId));
        }

        GroupId = groupId;
        SubjectId = subjectId;
        AcademicLoadId = academicLoadId;
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
