namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class Coordinator
{
    private Coordinator()
    {
    }

    public Coordinator(
        Guid tenantId,
        Guid personId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (personId == Guid.Empty)
        {
            throw new ArgumentException("La persona es obligatoria.", nameof(personId));
        }


        Id = Guid.NewGuid();
        TenantId = tenantId;
        PersonId = personId;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid PersonId { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(string academicDegree)
    {
        if (string.IsNullOrWhiteSpace(academicDegree))
        {
            throw new ArgumentException("El grado academico es obligatorio.", nameof(academicDegree));
        }

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