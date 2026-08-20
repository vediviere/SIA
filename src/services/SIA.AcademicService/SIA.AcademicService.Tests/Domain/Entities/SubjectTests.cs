using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Domain.Entities
{
    public class SubjectTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateSubject()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();


            // Act (ejecutar)
            var subject = new Subject(
                tenantId,
                "MAT-001",
                "Matemáticas",
                1,
                4,
                2,
                6);

            // Assert (Verificar)
            Assert.NotEqual(Guid.Empty, subject.Id);
            Assert.Equal(tenantId, subject.TenantId);
            Assert.Equal("MAT-001", subject.Code);
            Assert.Equal("Matemáticas", subject.Name);
            Assert.Equal(1, subject.Semester);
            Assert.Equal(4, subject.TheoryHours);
            Assert.Equal(2, subject.PracticeHours);
            Assert.Equal(6, subject.Credits);
            Assert.True(subject.Status);
        }
    }
}
