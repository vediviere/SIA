namespace SIA.AcademicService.Domain.Entities;
public sealed class StudyPlan
{
    private StudyPlan()
    {
    }

    public StudyPlan(
        Guid tenantId,
        Guid educationalProgramId,
        string code,
        string name,
        string version,
        DateOnly effectiveFrom)
    { 
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException(
                "El tenant es obligatorio.",
                nameof(tenantId));
        }

        if (educationalProgramId == Guid.Empty)
        {
            throw new ArgumentException(
                "El programa educacional es obligatorio.",
                nameof(educationalProgramId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "El codigo del plan de estudios es obligatorio.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "El nombre del plan de estudios es obligatorio.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace (version))
        {
            throw new ArgumentException(
                "La version del plan de estudios es obligatoria.",
                nameof(version));
        }

        if (effectiveFrom == default)
        {
            throw new ArgumentException(
                "La fecha de vigencia del plan de estudio es obligatoria.",
                nameof(effectiveFrom));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        EducationalProgramId = educationalProgramId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Version = version.Trim();
        EffectiveFrom = effectiveFrom;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid EducationalProgramId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Version { get; private set; } = string.Empty;
    public DateOnly EffectiveFrom {  get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

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

    public void UpdateDetails(string code, string name, string version, DateOnly effectiveFrom)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("El código del plan de estudios es obligatorio.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del plan de estudios es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("La versión del plan de estudios es obligatoria.", nameof(version));
        }

        if (effectiveFrom == default)
        {
            throw new ArgumentException("La fecha de vigencia del plan de estudios es obligatoria.", nameof(effectiveFrom));
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Version = version.Trim();
        EffectiveFrom = effectiveFrom;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
