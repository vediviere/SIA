using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace SIA.SchedulingService.Domain.Entities;

public sealed class Group
{
    private Group()
    {
    }

    public Group(
        Guid tenantId,
        Guid educationalProgramId,
        string groupName,
        string shift,
        int capacity)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (educationalProgramId == Guid.Empty)
        {
            throw new ArgumentException("El programa educativo es obligatorio.", nameof(educationalProgramId));
        }

        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new ArgumentException("El nombre del grupo es obligatorio.", nameof(groupName));
        }

        if (string.IsNullOrWhiteSpace(shift))
        {
            throw new ArgumentException("El turno del grupo es obligatorio.", nameof(shift));
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "El cupo debe ser mayor que cero.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        EducationalProgramId = educationalProgramId;
        GroupName = groupName.Trim().ToUpperInvariant();
        Shift = shift.Trim().ToUpperInvariant();
        Capacity = capacity;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid EducationalProgramId { get; private set; }

    public string GroupName { get; private set; } = string.Empty;

    public string Shift { get; private set; } = string.Empty;

    public int Capacity { get; private set; }

    public bool Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(string name,  string shift, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del grupo es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(shift))
        {
            throw new ArgumentException("El turno del grupo es obligatorio.", nameof(shift));
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "El cupo debe ser mayor que cero.");
        }
        GroupName = name.Trim().ToUpperInvariant();
        Shift = shift.Trim().ToUpperInvariant();
        Capacity = capacity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!Status) return;
        Status = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status) return;
        Status = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
