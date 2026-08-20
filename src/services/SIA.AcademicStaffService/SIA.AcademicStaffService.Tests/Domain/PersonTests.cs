using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Domain;

public sealed class PersonTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActivePerson()
    {
        var tenantId = Guid.NewGuid();

        var person = new Person(
            tenantId,
            " EMP-0001 ",
            " Ana ",
            " García ",
            " López ",
            " Maestría ",
            " ana.garcia@example.com ",
            " 7821234567 ");

        Assert.NotEqual(Guid.Empty, person.Id);
        Assert.Equal(tenantId, person.TenantId);
        Assert.Equal("EMP-0001", person.EmployeeNumber);
        Assert.Equal("Ana", person.FirstName);
        Assert.Equal("García", person.PaternalLastName);
        Assert.Equal("López", person.MaternalLastName);
        Assert.Equal("Maestría", person.AcademicDegree);
        Assert.Equal("ana.garcia@example.com", person.Email);
        Assert.Equal("7821234567", person.Phone);
        Assert.True(person.Status);
        Assert.Null(person.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.Empty, "EMP-0001", "Ana", "García", "López", "Maestría", "ana@example.com", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyEmployeeNumber_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "", "Ana", "García", "López", "Maestría", "ana@example.com", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "EMP-0001", "", "García", "López", "Maestría", "ana@example.com", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyPaternalLastName_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "EMP-0001", "Ana", "", "López", "Maestría", "ana@example.com", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyMaternalLastName_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "EMP-0001", "Ana", "García", "", "Maestría", "ana@example.com", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyAcademicDegree_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "", "ana@example.com", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyEmail_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "Maestría", "", "7821234567"));
    }

    [Fact]
    public void Constructor_WithEmptyPhone_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Person(
            Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "Maestría", "ana@example.com", ""));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFields()
    {
        var person = CreateValidPerson();

        person.Update("Ana María", "García", "López", "Doctorado", "nueva@example.com", "7820000000");

        Assert.Equal("Ana María", person.FirstName);
        Assert.Equal("Doctorado", person.AcademicDegree);
        Assert.Equal("nueva@example.com", person.Email);
        Assert.NotNull(person.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        var person = CreateValidPerson();

        Assert.Throws<ArgumentException>(() => person.Update("", "García", "López", "Doctorado", "nueva@example.com", "7820000000"));
    }

    [Fact]
    public void Deactivate_ShouldSetStatusFalse()
    {
        var person = CreateValidPerson();

        person.Deactivate();

        Assert.False(person.Status);
        Assert.NotNull(person.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_ShouldSetStatusTrue()
    {
        var person = CreateValidPerson();
        person.Deactivate();

        person.Activate();

        Assert.True(person.Status);
        Assert.NotNull(person.UpdatedAtUtc);
    }

    private static Person CreateValidPerson()
    {
        return new Person(
            Guid.NewGuid(), "EMP-0001", "Ana", "García", "López", "Maestría", "ana@example.com", "7821234567");
    }
}