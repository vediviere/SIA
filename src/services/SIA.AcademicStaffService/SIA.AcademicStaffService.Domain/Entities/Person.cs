namespace SIA.AcademicStaffService.Domain.Entities;

public sealed class Person
{
    private Person()
    {
    }

    public Person(
        Guid tenantId,
        string employeeNumber,
        string firstName,
        string paternalLastName,
        string maternalLastName,
        string academicDegree,
        string email,
        string phone)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            throw new ArgumentException("El número de empleado es obligatorio.", nameof(employeeNumber));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(paternalLastName))
        {
            throw new ArgumentException("El apellido paterno es obligatorio.", nameof(paternalLastName));
        }

        if (string.IsNullOrWhiteSpace(maternalLastName))
        {
            throw new ArgumentException("El apellido materno es obligatorio.", nameof(maternalLastName));
        }

        if (string.IsNullOrWhiteSpace(academicDegree))
        {
            throw new ArgumentException("El grado academico es obligatorio.", nameof(academicDegree));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El correo es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("El teléfono es obligatorio.", nameof(phone));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        EmployeeNumber = employeeNumber.Trim();
        FirstName = firstName.Trim();
        PaternalLastName = paternalLastName.Trim();
        MaternalLastName = maternalLastName.Trim();
        AcademicDegree = academicDegree.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string PaternalLastName { get; private set; } = string.Empty;
    public string MaternalLastName { get; private set; } = string.Empty;
    public string AcademicDegree { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(
        string firstName,
        string paternalLastName,
        string maternalLastName,
        string academicDegree,
        string email,
        string phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(paternalLastName))
        {
            throw new ArgumentException("El apellido paterno es obligatorio.", nameof(paternalLastName));
        }

        if (string.IsNullOrWhiteSpace(maternalLastName))
        {
            throw new ArgumentException("El apellido materno es obligatorio.", nameof(maternalLastName));
        }

        if (string.IsNullOrWhiteSpace(academicDegree))
        {
            throw new ArgumentException("El grado academico es obligatorio.", nameof(academicDegree));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El correo es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("El teléfono es obligatorio.", nameof(phone));
        }

        FirstName = firstName.Trim();
        PaternalLastName = paternalLastName.Trim();
        MaternalLastName = maternalLastName.Trim();
        AcademicDegree = academicDegree.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
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