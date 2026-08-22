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
            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

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
            Assert.Null(subject.UpdatedAtUtc);
        }

        [Fact]
        public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
        {
            // Arrange (preparar)
            var tenantId = Guid.Empty;

            // Act & Assert (ejecutar y verficar)
            Assert.Throws<ArgumentException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithEmptyCode_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Subject(tenantId, code, "Matemáticas", 1, 4, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithWhitespaceCode_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Subject(tenantId, code, "Matemáticas", 1, 4, 2, 6))
            ;
        }

        [Fact]
        public void Constructor_WithCodeContainingSpaces_ShouldNormalizeCode()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var code = "  mat-001  ";

            // Act (ejecutar)
            var subject = new Subject(tenantId, code, "Matemáticas", 1, 4, 2, 6);

            // Assert (verificar)
            Assert.Equal("MAT-001", subject.Code);
        }

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var name = string.Empty;

            //Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentException>(() =>
                new Subject(tenantId, "MAT-001", name, 1, 4, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithWhitespaceName_ShouldThrowArgumentException()
        {
            //Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var name = "   ";

            // Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentException>(() =>
                new Subject(tenantId, "MAT-001", name, 1, 4, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithNameContainingSpaces_ShouldNormalizeName()
        {
            //Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var name = "  Matemáticas  ";
            var code = "MAT-001";

            //Act (ejecutar)
            var subject = new Subject(tenantId, code, name, 1, 4, 2, 6);

            //Assert (verificar)
            Assert.Equal("Matemáticas", subject.Name);
        }

        [Fact]
        public void Constructor_WithZeroSemester_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var semester = 0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", semester, 4, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithNegativeSemester_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var semester = -1;

            // Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", semester, 4, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithNegativeTheoryHours_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange (oreparar)
            var tenantId = Guid.NewGuid();
            var theoryHours = -1;

            // Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", 1, theoryHours, 2, 6)
            );
        }

        [Fact]
        public void Constructor_WithNegativePracticeHours_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var practiceHours = -1;

            // Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, practiceHours, 6)
            );
        }

        [Fact]
        public void Constructor_WithNegativeCredits_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var credits = -1;

            // Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, credits));
        }

        [Fact]
        public void Constructor_WithZeroCredits_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange (preparar)
            var tenantId = Guid.NewGuid();
            var credits = 0;

            // Act & Assert (ejecutar y verificar)
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, credits));
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateSubject()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act
            subject.Update("MAT-002", "Cálculo", 2, 5, 3, 8);

            // Assert
            Assert.Equal("MAT-002", subject.Code);
            Assert.Equal("Cálculo", subject.Name);
            Assert.Equal(2, subject.Semester);
            Assert.Equal(5, subject.TheoryHours);
            Assert.Equal(3, subject.PracticeHours);
            Assert.Equal(8, subject.Credits);
        }

        [Fact]
        public void Update_WithUnnormalizedData_ShouldNormalizeSubject()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act
            subject.Update("  mat-002  ", "  Cálculo  ", 2, 5, 3, 8);

            // Assert
            Assert.Equal("MAT-002", subject.Code);
            Assert.Equal("Cálculo", subject.Name);
        }

        [Fact]
        public void Update_WithValidData_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            var beforeUpdate = DateTime.UtcNow;

            // Act
            subject.Update("MAT-002", "Cálculo", 2, 5, 3, 8);

            var afterUpdate = DateTime.UtcNow;

            // Assert
            Assert.NotNull(subject.UpdatedAtUtc);
            Assert.True(subject.UpdatedAtUtc.Value >= beforeUpdate);
            Assert.True(subject.UpdatedAtUtc.Value <= afterUpdate);
        }

        [Fact]
        public void Update_WithValidData_ShouldPreserveCreatedAtUtc()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            var createdAt = subject.CreatedAtUtc;

            // Act
            subject.Update("MAT-002", "Cálculo", 2, 5, 3, 8);

            // Assert
            Assert.Equal(createdAt, subject.CreatedAtUtc);
        }

        [Fact]
        public void Update_WithEmptyCode_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = string.Empty;

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                subject.Update(code, "Cálculo", 2, 5, 3, 8));
        }

        [Fact]
        public void Update_WithWhitespaceCode_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = "   ";

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                subject.Update(code, "Cálculo", 2, 5, 3, 8));
        }

        [Fact]
        public void Update_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = string.Empty;

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                subject.Update("MAT-001", name, 1, 2, 2, 6));
        }

        [Fact]
        public void Update_WithWhitespaceName_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "   ";

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                subject.Update("MAT-001", name, 1, 2, 2, 6));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_WithInvalidSemester_ShouldThrowArgumentOutOfRangeException(int semester)
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                subject.Update("MAT-001", "Matemáticas", semester, 2, 2, 6));
        }

        [Fact]
        public void Update_WithNegativeTheoryHours_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var theoryHours = -1;

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                subject.Update("MAT-001", "Matemáticas", 1, theoryHours, 2, 6));
        }

        [Fact]
        public void Update_WithNegativePracticeHours_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var practiceHours = -1;

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                subject.Update("MAT-001", "Matemáticas", 1, 4, practiceHours, 6));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_WithInvalidCredits_ShouldThrowArgumentOutOfRangeException(int credits)
        {
            //Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            //Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                subject.Update("MAT-001", "Matemáticas", 1, 2, 2, credits));

        }

        [Fact]
        public void SoftDelete_ShouldSetStatusToFalse()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act
            subject.SoftDelete();

            // Assert
            Assert.False(subject.Status);

        }

        [Fact]
        public void SoftDelete_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

            // Act
            subject.SoftDelete();

            // Assert
            Assert.NotNull(subject.UpdatedAtUtc);
        }

        [Fact]
        public void Restore_ShouldSetStatusToTrue()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);
            subject.SoftDelete();

            // Act
            subject.Restore();

            // Assert
            Assert.True(subject.Status);

        }

        [Fact]
        public void Restore_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);
            subject.SoftDelete();

            // Act
            subject.Restore();

            // Assert
            Assert.NotNull(subject.UpdatedAtUtc);
        }
    }
}
