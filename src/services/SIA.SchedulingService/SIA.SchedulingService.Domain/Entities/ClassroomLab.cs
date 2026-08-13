
namespace SIA.SchedulingService.Domain.Entities;

public sealed class ClassroomLab
{
    private ClassroomLab()
    {
    }

    public ClassroomLab(
        Guid tenantId,
        Guid buildingId,
        Guid classroomTypeId,
        string code,
        string name,
        int capacity,
        string description)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }
        if (buildingId == Guid.Empty)
        {
            throw new ArgumentException("El edificio es obligatorio.", nameof(buildingId));
        }
        if (classroomTypeId == Guid.Empty)
        {
            throw new ArgumentException("El tipo de aula es obligatorio.", nameof(classroomTypeId));
        }
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("La clave es obligatoria.", nameof(code));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "La capacidad debe ser mayor a cero.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        BuildingId = buildingId;
        ClassroomTypeId = classroomTypeId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Capacity = capacity;
        Description = description?.Trim() ?? string.Empty;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid BuildingId { get; private set; }
    public Guid ClassroomTypeId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public ClassroomType? ClassroomType { get; private set; }
    public Building? Building { get; private set; }

    public void Update( string code, string name, int capacity, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("La clave es obligatoria.", nameof(code));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "La capacidad debe ser mayor a cero.");
        }


        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Capacity = capacity;
        Description = description?.Trim() ?? string.Empty;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        Status = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        Status = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}