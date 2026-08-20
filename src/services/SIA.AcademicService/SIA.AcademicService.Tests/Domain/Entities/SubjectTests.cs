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

        [Fact]
        public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
        {
            // Arrange (preparar)
            var tenantId = Guid.Empty;

            // Act & Assert (ejecutar y verficar)
            Assert.Throws<ArgumentException>(() =>
                new Subject(
                    tenantId,
                    "MAT-001",
                    "Matemáticas",
                    1,
                    4,
                    2,
                    6));
        }

        [Fact]
        public void Constructor_WithEmptyCode_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Subject(
                    tenantId,
                    "   ",
                    "Matemáticas",
                    1,
                    4,
                    2,
                    6));
        }

        [Fact]
        public void Constructor_WithWhitespaceCode_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Subject(
                    tenantId,
                    code,
                    "Matemáticas",
                    1,
                    4,
                    2,
                    6));
        }

        [Fact]
        public void Constructor_WithCodeContainingSpaces_ShouldNormalizeCode()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = "  mat-001  ";

            // Act
            var subject = new Subject(
                tenantId,
                code,
                "Matemáticas",
                1,
                4,
                2,
                6);

            // Assert
            Assert.Equal("MAT-001", subject.Code);
        }

    }
}
