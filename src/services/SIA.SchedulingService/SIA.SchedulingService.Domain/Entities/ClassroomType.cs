namespace SIA.SchedulingService.Domain.Entities;

public sealed class ClassroomType
{
    private ClassroomType()
    {
    }

    public ClassroomType(
        Guid tenantId,
        string code,
        string name,
        string description)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("La clave es obligatoria.", nameof(code));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del tipo es obligatorio.", nameof(name));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(string code, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("La clave es obligatoria.", nameof(code));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del tipo es obligatorio.", nameof(name));
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
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