namespace SIA.AcademicService.Domain.Entities;

public sealed class EducationalProgram
{
    private EducationalProgram()
    {
    }

    public EducationalProgram(
        Guid tenantId,
        string code,
        string name,
        string level)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException(
                "El tenant es obligatorio.",
                nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "El código del programa educacional es obligatorio.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "El nombre del programa educacional es obligatorio.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(level))
        {
            throw new ArgumentException(
                "Debe de asignar un nivel educativo.",
                nameof(level));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Level = level.Trim();
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Level { get; private set; } = string.Empty;
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Desactivate()
    {
        Status = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(string code, string name, string level)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("El código del programa educacional es obligatorio.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del programa educacional es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(level))
        {
            throw new ArgumentException("Debe de asignar un nivel educativo.", nameof(level));
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Level = level.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}