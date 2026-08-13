namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class DivisionHead
{
    private DivisionHead()
    {
    }

    public DivisionHead(
        Guid tenantId,
        Guid programId,
        Guid personId,
        string academicDegree)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (programId == Guid.Empty)
        {
            throw new ArgumentException("El programa es obligatorio.", nameof(programId));
        }

        if (personId == Guid.Empty)
        {
            throw new ArgumentException("La persona es obligatoria.", nameof(personId));
        }

        if (string.IsNullOrWhiteSpace(academicDegree))
        {
            throw new ArgumentException("El grado academico es obligatorio.", nameof(academicDegree));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProgramId = programId;
        PersonId = personId;
        AcademicDegree = academicDegree.Trim();
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid PersonId { get; private set; }
    public string AcademicDegree { get; private set; } = string.Empty;
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

    public void Update(string academicDegree)
    {
        if (string.IsNullOrWhiteSpace(academicDegree))
        {
            throw new ArgumentException("El grado academico es obligatorio.", nameof(academicDegree));
        }

        AcademicDegree = academicDegree.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}