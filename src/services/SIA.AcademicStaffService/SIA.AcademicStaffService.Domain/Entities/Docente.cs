using System.Runtime.InteropServices;

namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class Docente
{
    private Docente()
    {
    }

    public Docente(
        Guid tenantId,
        Guid personaId,
        string gradoAcademico,
        string perfilProfesional,
        string tipoContrato,
        int horasContrato)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.",nameof(tenantId));
        }

        if (personaId == Guid.Empty)
        {
            throw new ArgumentException("El personaId es obligatorio.", nameof(personaId));
        }

        if (string.IsNullOrWhiteSpace(gradoAcademico))
        {
            throw new ArgumentException("El grado academico es obligatorio.",nameof(gradoAcademico));
        }

        if (string.IsNullOrWhiteSpace(perfilProfesional))
        {
            throw new ArgumentException("El perfil profesional es obligatorio.",nameof(perfilProfesional));
        }

        if (string.IsNullOrWhiteSpace(tipoContrato))
        {
            throw new ArgumentException("El tipo de contrato es obligatorio.",nameof(tipoContrato));
        }

        if (horasContrato <= 0)
        {
            throw new ArgumentOutOfRangeException("Las horas de contrato deben ser mayor a cero",nameof(horasContrato));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        PersonaId = personaId;
        GradoAcademico = gradoAcademico.Trim();
        PerfilProfesional = perfilProfesional.Trim();
        TipoContrato = tipoContrato.Trim();
        HorasContrato = horasContrato;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow; 
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid PersonaId { get; private set; }
    public string GradoAcademico { get; private set; } = string.Empty;
    public string PerfilProfesional { get; private set; } = string.Empty;
    public string TipoContrato { get; private set; } = string.Empty;
    public int HorasContrato { get; private set; }
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
