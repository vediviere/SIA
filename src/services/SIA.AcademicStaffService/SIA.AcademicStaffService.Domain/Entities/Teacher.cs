namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class Teacher
{
    private Teacher()
    {
    }

    public Teacher(
        Guid tenantId,
        Guid personId,
        string professionalProfile,
        string contractType,
        int contractHours,
        Guid? programId = null)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (personId == Guid.Empty)
        {
            throw new ArgumentException("El personId es obligatorio.", nameof(personId));
        }

        if (string.IsNullOrWhiteSpace(professionalProfile))
        {
            throw new ArgumentException("El perfil profesional es obligatorio.", nameof(professionalProfile));
        }

        if (string.IsNullOrWhiteSpace(contractType))
        {
            throw new ArgumentException("El tipo de contrato es obligatorio.", nameof(contractType));
        }

        if (contractHours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(contractHours), "Las horas de contrato deben ser mayor a cero.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        PersonId = personId;
        ProfessionalProfile = professionalProfile.Trim();
        ContractType = contractType.Trim();
        ContractHours = contractHours;
        ProgramId = programId;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid PersonId { get; private set; }
    public string ProfessionalProfile { get; private set; } = string.Empty;
    public string ContractType { get; private set; } = string.Empty;
    public int ContractHours { get; private set; }
    public Guid? ProgramId { get; private set; }
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

    public void Update(
        string professionalProfile,
        string contractType,
        int contractHours)
    {

        if (string.IsNullOrWhiteSpace(professionalProfile))
        {
            throw new ArgumentException("El perfil profesional es obligatorio.", nameof(professionalProfile));
        }

        if (string.IsNullOrWhiteSpace(contractType))
        {
            throw new ArgumentException("El tipo de contrato es obligatorio.", nameof(contractType));
        }

        if (contractHours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(contractHours), "Las horas de contrato deben ser mayor a cero.");
        }

        ProfessionalProfile = professionalProfile.Trim();
        ContractType = contractType.Trim();
        ContractHours = contractHours;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AssignProgram(Guid? programId)
    {
        ProgramId = programId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}