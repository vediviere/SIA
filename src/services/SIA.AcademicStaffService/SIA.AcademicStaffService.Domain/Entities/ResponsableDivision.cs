using System.Net.NetworkInformation;

namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class ResponsableDivision
{
    private ResponsableDivision()
    {
    }

    public ResponsableDivision(
        Guid tenantId,
        Guid programaId,
        Guid personaId,
        string gradoAcademico)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.",nameof(tenantId));
        }

        if (programaId == Guid.Empty)
        {
            throw new ArgumentException("El programa es obligatorio.",nameof(programaId));
        }

        if (personaId == Guid.Empty)
        {
            throw new ArgumentException("La persona es obligatoria.",nameof(personaId));
        }

        if (string.IsNullOrWhiteSpace(gradoAcademico))
        {
            throw new ArgumentException("El grado acaemico es obligatorio.", nameof(gradoAcademico));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProgramaId = programaId;
        PersonaId = personaId;
        GradoAcademico = gradoAcademico.Trim();
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProgramaId { get; private set; }
    public Guid PersonaId { get; private set; }
    public string GradoAcademico { get; private set; } = string.Empty;
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
}
