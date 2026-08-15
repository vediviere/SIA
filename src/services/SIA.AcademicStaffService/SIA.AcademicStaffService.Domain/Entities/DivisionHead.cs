namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class DivisionHead
{
    private DivisionHead()
    {
    }

    public DivisionHead(
        Guid tenantId,
        Guid programId,
        Guid personId)
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

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProgramId = programId;
        PersonId = personId;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid PersonId { get; private set; }
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

        UpdatedAtUtc = DateTime.UtcNow;
    }
}